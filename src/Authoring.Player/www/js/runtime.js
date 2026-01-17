/**
 * SlideForge Runtime Player
 * JavaScript runtime engine for executing SlideForge projects
 */

// ============================================================================
// Project Loader
// ============================================================================

class ProjectLoader {
    /**
     * Load and parse a project JSON file
     * @param {string} jsonPath - Path to the project JSON file
     * @returns {Promise<Object>} The parsed project object
     */
    async loadProject(jsonPath) {
        try {
            const response = await fetch(jsonPath);
            if (!response.ok) {
                throw new Error(`Failed to load project: ${response.status} ${response.statusText}`);
            }
            const json = await response.text();
            const project = JSON.parse(json);
            
            // Basic validation
            if (!project.id || !project.name || !Array.isArray(project.slides)) {
                throw new Error('Invalid project structure: missing required fields');
            }
            
            return project;
        } catch (error) {
            throw new Error(`Error loading project: ${error.message}`);
        }
    }
}

// ============================================================================
// Variable System
// ============================================================================

class VariableSystem {
    constructor() {
        this.variables = new Map();
        this.changeListeners = [];
    }

    /**
     * Initialize variables from project defaults
     * @param {Array} projectVariables - Array of variable definitions from project
     */
    initialize(projectVariables) {
        this.variables.clear();
        for (const variable of projectVariables || []) {
            this.variables.set(variable.id, {
                type: variable.type,
                name: variable.name,
                value: variable.defaultValue
            });
        }
    }

    /**
     * Get a variable value
     * @param {string} variableId - ID of the variable
     * @returns {*} The variable value
     */
    get(variableId) {
        const variable = this.variables.get(variableId);
        return variable ? variable.value : undefined;
    }

    /**
     * Set a variable value with type validation
     * @param {string} variableId - ID of the variable
     * @param {*} value - The value to set
     */
    set(variableId, value) {
        const variable = this.variables.get(variableId);
        if (!variable) {
            console.warn(`Variable not found: ${variableId}`);
            return;
        }

        // Type validation and conversion
        let validatedValue = value;
        switch (variable.type) {
            case 'Boolean':
                validatedValue = Boolean(value);
                break;
            case 'Number':
                validatedValue = Number(value);
                if (isNaN(validatedValue)) {
                    console.warn(`Invalid number value for variable ${variableId}: ${value}`);
                    return;
                }
                break;
            case 'String':
                validatedValue = String(value);
                break;
        }

        variable.value = validatedValue;
        this.notifyChange(variableId, validatedValue);
    }

    /**
     * Subscribe to variable changes
     * @param {Function} listener - Callback function (variableId, newValue) => {}
     */
    onChange(listener) {
        this.changeListeners.push(listener);
    }

    /**
     * Notify all listeners of a variable change
     * @param {string} variableId - ID of the changed variable
     * @param {*} newValue - The new value
     */
    notifyChange(variableId, newValue) {
        for (const listener of this.changeListeners) {
            try {
                listener(variableId, newValue);
            } catch (error) {
                console.error(`Error in variable change listener: ${error}`);
            }
        }
    }
}

// ============================================================================
// Layer Manager
// ============================================================================

class LayerManager {
    constructor() {
        this.layers = new Map();
        this.visibleLayers = new Set();
    }

    /**
     * Initialize layers from a slide
     * @param {Array} layers - Array of layer definitions from slide
     */
    initialize(layers) {
        this.layers.clear();
        this.visibleLayers.clear();
        
        for (const layer of layers || []) {
            this.layers.set(layer.id, layer);
            if (layer.visible) {
                this.visibleLayers.add(layer.id);
            }
        }
    }

    /**
     * Show a layer
     * @param {string} layerId - ID of the layer to show
     */
    showLayer(layerId) {
        if (this.layers.has(layerId)) {
            this.visibleLayers.add(layerId);
        }
    }

    /**
     * Hide a layer
     * @param {string} layerId - ID of the layer to hide
     */
    hideLayer(layerId) {
        this.visibleLayers.delete(layerId);
    }

    /**
     * Check if a layer is visible
     * @param {string} layerId - ID of the layer to check
     * @returns {boolean} True if the layer is visible
     */
    isVisible(layerId) {
        return this.visibleLayers.has(layerId);
    }
}

// ============================================================================
// Object Renderer
// ============================================================================

class ObjectRenderer {
    /**
     * Render an object as a DOM element
     * @param {Object} object - The object to render (TextObject, ImageObject, or ButtonObject)
     * @param {HTMLElement} slideElement - The slide container element
     * @returns {HTMLElement} The created DOM element
     */
    renderObject(object, slideElement) {
        let element;
        
        switch (object.objectType) {
            case 'text':
                element = this.renderTextObject(object);
                break;
            case 'image':
                element = this.renderImageObject(object);
                break;
            case 'button':
                element = this.renderButtonObject(object);
                break;
            default:
                console.warn(`Unknown object type: ${object.objectType}`);
                return null;
        }

        if (!element) return null;

        // Apply common properties
        element.className = `slide-object ${object.objectType}-object`;
        element.style.position = 'absolute';
        element.style.left = `${object.x}px`;
        element.style.top = `${object.y}px`;
        element.style.width = `${object.width}px`;
        element.style.height = `${object.height}px`;
        element.style.display = object.visible !== false ? 'flex' : 'none';
        
        // Store object reference for trigger lookup
        element.dataset.objectId = object.id;
        element.dataset.objectType = object.objectType;

        return element;
    }

    /**
     * Render a TextObject
     * @param {Object} object - The TextObject definition
     * @returns {HTMLElement} The created div element
     */
    renderTextObject(object) {
        const element = document.createElement('div');
        element.textContent = object.text || '';
        element.style.fontFamily = object.fontFamily || 'Arial';
        element.style.fontSize = `${object.fontSize || 12}pt`;
        element.style.color = object.color || '#000000';
        return element;
    }

    /**
     * Render an ImageObject
     * @param {Object} object - The ImageObject definition
     * @returns {HTMLElement} The created img element container
     */
    renderImageObject(object) {
        const container = document.createElement('div');
        const img = document.createElement('img');
        img.src = object.sourcePath || '';
        img.alt = object.name || 'Image';
        
        if (object.maintainAspectRatio !== false) {
            img.classList.add('maintain-aspect');
        }
        
        container.appendChild(img);
        return container;
    }

    /**
     * Render a ButtonObject
     * @param {Object} object - The ButtonObject definition
     * @returns {HTMLElement} The created button element
     */
    renderButtonObject(object) {
        const element = document.createElement('button');
        element.textContent = object.label || 'Button';
        element.type = 'button';
        
        if (object.enabled === false) {
            element.classList.add('disabled');
            element.disabled = true;
        }
        
        return element;
    }
}

// ============================================================================
// Slide Renderer
// ============================================================================

class SlideRenderer {
    constructor(objectRenderer) {
        this.objectRenderer = objectRenderer;
    }

    /**
     * Render a complete slide with layers and objects
     * @param {Object} slide - The slide definition
     * @param {Object} project - The project definition
     * @param {LayerManager} layerManager - The layer manager instance
     * @returns {HTMLElement} The created slide container element
     */
    renderSlide(slide, project, layerManager) {
        const slideElement = document.createElement('div');
        slideElement.className = 'slide-container';
        slideElement.style.width = `${slide.width || 1920}px`;
        slideElement.style.height = `${slide.height || 1080}px`;

        // Render layers in order
        for (const layer of slide.layers || []) {
            const layerElement = this.renderLayer(layer, slideElement, layerManager);
            if (layerElement) {
                slideElement.appendChild(layerElement);
            }
        }

        return slideElement;
    }

    /**
     * Render a layer with its objects
     * @param {Object} layer - The layer definition
     * @param {HTMLElement} slideElement - The slide container
     * @param {LayerManager} layerManager - The layer manager instance
     * @returns {HTMLElement} The created layer container element
     */
    renderLayer(layer, slideElement, layerManager) {
        const layerElement = document.createElement('div');
        layerElement.className = 'layer';
        layerElement.dataset.layerId = layer.id;
        
        // Apply layer visibility
        if (!layerManager.isVisible(layer.id)) {
            layerElement.classList.add('hidden');
        }

        // Render objects in this layer
        for (const object of layer.objects || []) {
            const objectElement = this.objectRenderer.renderObject(object, slideElement);
            if (objectElement) {
                layerElement.appendChild(objectElement);
            }
        }

        return layerElement;
    }
}

// ============================================================================
// Timeline Engine
// ============================================================================

class TimelineEngine {
    constructor() {
        this.currentTime = 0;
        this.isPlaying = false;
        this.animationFrameId = null;
        this.lastUpdateTime = 0;
        this.onTimelineStartCallbacks = [];
    }

    /**
     * Start timeline playback
     */
    play() {
        if (this.isPlaying) return;
        
        this.isPlaying = true;
        this.lastUpdateTime = performance.now() / 1000;
        this.update();
    }

    /**
     * Pause timeline playback
     */
    pause() {
        this.isPlaying = false;
        if (this.animationFrameId) {
            cancelAnimationFrame(this.animationFrameId);
            this.animationFrameId = null;
        }
    }

    /**
     * Reset timeline to start
     */
    reset() {
        this.pause();
        this.currentTime = 0;
        this.lastUpdateTime = 0;
    }

    /**
     * Update timeline and object visibility
     * Called on animation frame when playing
     */
    update() {
        if (!this.isPlaying) return;

        const now = performance.now() / 1000;
        const deltaTime = now - this.lastUpdateTime;
        this.currentTime += deltaTime;
        this.lastUpdateTime = now;

        // Update object visibility based on timeline
        this.updateObjectVisibility();

        this.animationFrameId = requestAnimationFrame(() => this.update());
    }

    /**
     * Update visibility of objects based on their timeline
     */
    updateObjectVisibility() {
        // This will be called by PlayerController with the current slide's objects
        // The actual implementation is handled in PlayerController.setupTimeline
    }

    /**
     * Check if an object should be visible at current time
     * @param {Object} object - The object to check
     * @returns {boolean} True if object should be visible
     */
    shouldBeVisible(object) {
        if (!object.timeline) {
            return object.visible !== false;
        }

        const timeline = object.timeline;
        return this.currentTime >= timeline.startTime && 
               this.currentTime < timeline.startTime + timeline.duration;
    }

    /**
     * Register callback for OnTimelineStart triggers
     * @param {Function} callback - Callback function (object, trigger) => {}
     */
    onTimelineStart(callback) {
        this.onTimelineStartCallbacks.push(callback);
    }

    /**
     * Notify that an object has become visible (fires OnTimelineStart triggers)
     * @param {Object} object - The object that became visible
     */
    notifyObjectVisible(object) {
        for (const callback of this.onTimelineStartCallbacks) {
            try {
                callback(object);
            } catch (error) {
                console.error(`Error in timeline start callback: ${error}`);
            }
        }
    }
}

// ============================================================================
// Trigger Evaluator
// ============================================================================

class TriggerEvaluator {
    /**
     * Evaluate if a trigger should fire
     * @param {Object} trigger - The trigger definition
     * @param {string} triggerType - The type of trigger event ('OnClick' or 'OnTimelineStart')
     * @param {Object} context - Context information (object, slide, etc.)
     * @returns {boolean} True if trigger should fire
     */
    evaluateTrigger(trigger, triggerType, context) {
        if (trigger.type !== triggerType) {
            return false;
        }

        // For now, triggers fire immediately when their type matches
        // Future: could add conditions (e.g., variable checks)
        return true;
    }
}

// ============================================================================
// Action Executor
// ============================================================================

class ActionExecutor {
    constructor(playerController, variableSystem, layerManager) {
        this.playerController = playerController;
        this.variableSystem = variableSystem;
        this.layerManager = layerManager;
    }

    /**
     * Execute a single action
     * @param {Object} action - The action definition
     * @param {Object} context - Context information
     */
    executeAction(action, context = {}) {
        switch (action.actionType) {
            case 'navigateToSlide':
                this.executeNavigateToSlide(action);
                break;
            case 'setVariable':
                this.executeSetVariable(action);
                break;
            case 'showLayer':
                this.executeShowLayer(action, context);
                break;
            case 'hideLayer':
                this.executeHideLayer(action, context);
                break;
            default:
                console.warn(`Unknown action type: ${action.actionType}`);
        }
    }

    /**
     * Execute NavigateToSlideAction
     * @param {Object} action - The NavigateToSlideAction definition
     */
    executeNavigateToSlide(action) {
        if (this.playerController) {
            this.playerController.navigateToSlide(action.targetSlideId);
        }
    }

    /**
     * Execute SetVariableAction
     * @param {Object} action - The SetVariableAction definition
     */
    executeSetVariable(action) {
        if (this.variableSystem) {
            this.variableSystem.set(action.variableId, action.value);
        }
    }

    /**
     * Execute ShowLayerAction
     * @param {Object} action - The ShowLayerAction definition
     * @param {Object} context - Context containing the current slide element
     */
    executeShowLayer(action, context) {
        if (this.layerManager) {
            this.layerManager.showLayer(action.layerId);
            
            // Update DOM if slide element is available
            if (context.slideElement) {
                const layerElement = context.slideElement.querySelector(`[data-layer-id="${action.layerId}"]`);
                if (layerElement) {
                    layerElement.classList.remove('hidden');
                }
            }
        }
    }

    /**
     * Execute HideLayerAction
     * @param {Object} action - The HideLayerAction definition
     * @param {Object} context - Context containing the current slide element
     */
    executeHideLayer(action, context) {
        if (this.layerManager) {
            this.layerManager.hideLayer(action.layerId);
            
            // Update DOM if slide element is available
            if (context.slideElement) {
                const layerElement = context.slideElement.querySelector(`[data-layer-id="${action.layerId}"]`);
                if (layerElement) {
                    layerElement.classList.add('hidden');
                }
            }
        }
    }
}

// ============================================================================
// Player Controller
// ============================================================================

class PlayerController {
    constructor() {
        this.currentSlideId = null;
        this.project = null;
        this.currentSlideElement = null;
        this.previouslyVisibleObjects = new Set();

        // Initialize subsystems
        this.variableSystem = new VariableSystem();
        this.layerManager = new LayerManager();
        this.timelineEngine = new TimelineEngine();
        this.objectRenderer = new ObjectRenderer();
        this.slideRenderer = new SlideRenderer(this.objectRenderer);
        this.triggerEvaluator = new TriggerEvaluator();
        this.actionExecutor = new ActionExecutor(this, this.variableSystem, this.layerManager);

        // Setup timeline callbacks
        this.timelineEngine.onTimelineStart((object) => {
            this.handleTimelineStart(object);
        });
    }

    /**
     * Initialize the player with a project
     * @param {string} projectJsonPath - Path to the project JSON file
     */
    async initialize(projectJsonPath) {
        const loader = new ProjectLoader();
        this.project = await loader.loadProject(projectJsonPath);
        
        // Initialize variable system
        this.variableSystem.initialize(this.project.variables);
        
        // Start with first slide
        if (this.project.slides && this.project.slides.length > 0) {
            this.currentSlideId = this.project.slides[0].id;
        }
    }

    /**
     * Navigate to a specific slide
     * @param {string} slideId - ID of the slide to navigate to
     */
    navigateToSlide(slideId) {
        const slide = this.project.slides.find(s => s.id === slideId);
        if (!slide) {
            console.warn(`Slide not found: ${slideId}`);
            return;
        }

        this.currentSlideId = slideId;
        this.renderCurrentSlide();
    }

    /**
     * Render the current slide
     */
    renderCurrentSlide() {
        const slide = this.project.slides.find(s => s.id === this.currentSlideId);
        if (!slide) return;

        // Initialize layer manager for this slide
        this.layerManager.initialize(slide.layers);

        // Get slide container from DOM
        const slideContainer = document.getElementById('slide-container');
        if (!slideContainer) return;

        // Clear previous slide
        slideContainer.innerHTML = '';

        // Render new slide
        this.currentSlideElement = this.slideRenderer.renderSlide(slide, this.project, this.layerManager);
        slideContainer.appendChild(this.currentSlideElement);

        // Setup triggers and timeline
        this.setupTriggers();
        this.setupTimeline();

        // Reset timeline
        this.timelineEngine.reset();
        this.previouslyVisibleObjects.clear();
    }

    /**
     * Setup trigger listeners for the current slide
     */
    setupTriggers() {
        if (!this.currentSlideElement) return;

        const slide = this.project.slides.find(s => s.id === this.currentSlideId);
        if (!slide) return;

        // Find all objects with triggers
        for (const layer of slide.layers || []) {
            for (const object of layer.objects || []) {
                // Setup OnClick triggers for buttons
                if (object.objectType === 'button' && object.triggers) {
                    const buttonElement = this.currentSlideElement.querySelector(`[data-object-id="${object.id}"]`);
                    if (buttonElement) {
                        buttonElement.addEventListener('click', () => {
                            this.handleObjectClick(object);
                        });
                    }
                }
            }
        }
    }

    /**
     * Handle click on an object (button)
     * @param {Object} object - The object that was clicked
     */
    handleObjectClick(object) {
        if (object.objectType === 'button' && object.enabled === false) {
            return; // Disabled buttons don't trigger
        }

        // Find OnClick triggers for this object
        for (const trigger of object.triggers || []) {
            if (this.triggerEvaluator.evaluateTrigger(trigger, 'OnClick', { object })) {
                this.executeTrigger(trigger);
            }
        }
    }

    /**
     * Handle timeline start event for an object
     * @param {Object} object - The object that became visible
     */
    handleTimelineStart(object) {
        // Find OnTimelineStart triggers for this object
        for (const trigger of object.triggers || []) {
            if (this.triggerEvaluator.evaluateTrigger(trigger, 'OnTimelineStart', { object })) {
                this.executeTrigger(trigger);
            }
        }
    }

    /**
     * Execute all actions for a trigger
     * @param {Object} trigger - The trigger to execute
     */
    executeTrigger(trigger) {
        const context = {
            slideElement: this.currentSlideElement,
            trigger: trigger
        };

        for (const action of trigger.actions || []) {
            this.actionExecutor.executeAction(action, context);
        }
    }

    /**
     * Setup timeline for the current slide
     */
    setupTimeline() {
        if (!this.currentSlideElement) return;

        const slide = this.project.slides.find(s => s.id === this.currentSlideId);
        if (!slide) return;

        // Store reference to objects for timeline updates
        this.slideObjects = [];
        for (const layer of slide.layers || []) {
            for (const object of layer.objects || []) {
                this.slideObjects.push(object);
            }
        }

        // Update object visibility based on timeline
        this.updateTimelineVisibility();
    }

    /**
     * Update object visibility based on timeline
     */
    updateTimelineVisibility() {
        if (!this.currentSlideElement || !this.slideObjects) return;

        for (const object of this.slideObjects) {
            const shouldBeVisible = this.timelineEngine.shouldBeVisible(object);
            const wasVisible = this.previouslyVisibleObjects.has(object.id);
            const element = this.currentSlideElement.querySelector(`[data-object-id="${object.id}"]`);

            if (element) {
                if (shouldBeVisible && object.visible !== false) {
                    element.style.display = '';
                    
                    // Fire OnTimelineStart if object just became visible
                    if (!wasVisible && object.timeline) {
                        this.previouslyVisibleObjects.add(object.id);
                        this.timelineEngine.notifyObjectVisible(object);
                    }
                } else {
                    element.style.display = 'none';
                    this.previouslyVisibleObjects.delete(object.id);
                }
            }
        }
    }

    /**
     * Start the player
     */
    start() {
        this.renderCurrentSlide();
    }

    /**
     * Get the current slide
     * @returns {Object} The current slide definition
     */
    getCurrentSlide() {
        return this.project.slides.find(s => s.id === this.currentSlideId);
    }

    /**
     * Get current slide index
     * @returns {number} The index of the current slide (0-based)
     */
    getCurrentSlideIndex() {
        return this.project.slides.findIndex(s => s.id === this.currentSlideId);
    }
}

// ============================================================================
// Player UI Controller
// ============================================================================

class PlayerUIController {
    constructor(playerController) {
        this.playerController = playerController;
        this.timelineUpdateLoopId = null;
        this.setupUI();
    }

    setupUI() {
        // Get UI elements
        this.loadingElement = document.getElementById('loading');
        this.errorElement = document.getElementById('error');
        this.errorMessageElement = document.getElementById('error-message');
        this.playerElement = document.getElementById('player');
        this.prevButton = document.getElementById('btn-prev');
        this.nextButton = document.getElementById('btn-next');
        this.playPauseButton = document.getElementById('btn-play-pause');
        this.slideTitleElement = document.getElementById('slide-title');
        this.slideProgressElement = document.getElementById('slide-progress');

        // Setup event listeners
        this.prevButton.addEventListener('click', () => this.navigatePrevious());
        this.nextButton.addEventListener('click', () => this.navigateNext());
        this.playPauseButton.addEventListener('click', () => this.togglePlayPause());

        // Update UI periodically when timeline is playing
        setInterval(() => this.updateUI(), 100);
    }

    navigatePrevious() {
        const currentIndex = this.playerController.getCurrentSlideIndex();
        if (currentIndex > 0) {
            const prevSlide = this.playerController.project.slides[currentIndex - 1];
            this.playerController.navigateToSlide(prevSlide.id);
        }
    }

    navigateNext() {
        const currentIndex = this.playerController.getCurrentSlideIndex();
        const totalSlides = this.playerController.project.slides.length;
        if (currentIndex < totalSlides - 1) {
            const nextSlide = this.playerController.project.slides[currentIndex + 1];
            this.playerController.navigateToSlide(nextSlide.id);
        }
    }

    togglePlayPause() {
        if (this.playerController.timelineEngine.isPlaying) {
            this.playerController.timelineEngine.pause();
            // Cancel timeline update loop
            if (this.timelineUpdateLoopId !== null) {
                cancelAnimationFrame(this.timelineUpdateLoopId);
                this.timelineUpdateLoopId = null;
            }
            this.playPauseButton.textContent = '▶ Play';
        } else {
            this.playerController.timelineEngine.play();
            // Cancel any existing loop before starting new one
            if (this.timelineUpdateLoopId !== null) {
                cancelAnimationFrame(this.timelineUpdateLoopId);
            }
            // Update timeline visibility continuously
            const updateLoop = () => {
                if (this.playerController.timelineEngine.isPlaying) {
                    this.playerController.updateTimelineVisibility();
                    this.timelineUpdateLoopId = requestAnimationFrame(updateLoop);
                } else {
                    this.timelineUpdateLoopId = null;
                }
            };
            this.timelineUpdateLoopId = requestAnimationFrame(updateLoop);
            this.playPauseButton.textContent = '⏸ Pause';
        }
    }

    updateUI() {
        const currentSlide = this.playerController.getCurrentSlide();
        const currentIndex = this.playerController.getCurrentSlideIndex();
        const totalSlides = this.playerController.project.slides.length;

        // Update slide title
        if (this.slideTitleElement && currentSlide) {
            this.slideTitleElement.textContent = currentSlide.title || 'Untitled Slide';
        }

        // Update progress
        if (this.slideProgressElement) {
            this.slideProgressElement.textContent = `${currentIndex + 1} / ${totalSlides}`;
        }

        // Update button states
        if (this.prevButton) {
            this.prevButton.disabled = currentIndex === 0;
        }
        if (this.nextButton) {
            this.nextButton.disabled = currentIndex >= totalSlides - 1;
        }

        // Update play/pause button
        if (this.playPauseButton) {
            if (this.playerController.timelineEngine.isPlaying) {
                this.playPauseButton.textContent = '⏸ Pause';
            } else {
                this.playPauseButton.textContent = '▶ Play';
            }
        }
    }

    showLoading() {
        if (this.loadingElement) this.loadingElement.style.display = 'flex';
        if (this.errorElement) this.errorElement.style.display = 'none';
        if (this.playerElement) this.playerElement.style.display = 'none';
    }

    showError(message) {
        if (this.loadingElement) this.loadingElement.style.display = 'none';
        if (this.errorElement) {
            this.errorElement.style.display = 'flex';
            if (this.errorMessageElement) {
                this.errorMessageElement.textContent = message;
            }
        }
        if (this.playerElement) this.playerElement.style.display = 'none';
    }

    showPlayer() {
        if (this.loadingElement) this.loadingElement.style.display = 'none';
        if (this.errorElement) this.errorElement.style.display = 'none';
        if (this.playerElement) this.playerElement.style.display = 'flex';
    }
}

// ============================================================================
// Initialize Player
// ============================================================================

document.addEventListener('DOMContentLoaded', async () => {
    const uiController = new PlayerUIController(new PlayerController());
    const playerController = uiController.playerController;

    try {
        uiController.showLoading();

        // Get project path from URL parameter
        const urlParams = new URLSearchParams(window.location.search);
        const projectPath = urlParams.get('project') || 'project.json';

        // Initialize player
        await playerController.initialize(projectPath);
        playerController.start();

        // Show player
        uiController.showPlayer();
    } catch (error) {
        console.error('Failed to initialize player:', error);
        uiController.showError(error.message);
    }
});