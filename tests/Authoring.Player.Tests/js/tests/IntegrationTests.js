/**
 * Integration tests for Player runtime
 */

function runIntegrationTests(testRunner, assert) {
    testRunner.describe('Integration Tests', () => {
        
        testRunner.it('PlayerController_LoadProjectWithVariables_InitializesVariables', async () => {
            // Arrange
            const projectJson = {
                id: 'test-project',
                name: 'Test Project',
                slides: [{
                    id: 'slide-1',
                    title: 'Slide 1',
                    width: 1920,
                    height: 1080,
                    layers: [{
                        id: 'layer-1',
                        name: 'Base Layer',
                        visible: true,
                        objects: []
                    }]
                }],
                variables: [
                    { id: 'var1', name: 'Var 1', type: 'Boolean', defaultValue: true },
                    { id: 'var2', name: 'Var 2', type: 'Number', defaultValue: 42 }
                ]
            };
            
            global.fetch = async () => ({
                ok: true,
                text: async () => JSON.stringify(projectJson)
            });
            
            const controller = new PlayerController();
            
            // Act
            await controller.initialize('project.json');
            
            // Assert
            assert.equal(controller.variableSystem.get('var1'), true);
            assert.equal(controller.variableSystem.get('var2'), 42);
        });
        
        testRunner.it('PlayerController_NavigateToSlide_ChangesCurrentSlide', async () => {
            // Arrange
            const projectJson = {
                id: 'test-project',
                name: 'Test Project',
                slides: [
                    {
                        id: 'slide-1',
                        title: 'Slide 1',
                        width: 1920,
                        height: 1080,
                        layers: [{ id: 'layer-1', name: 'Base', visible: true, objects: [] }]
                    },
                    {
                        id: 'slide-2',
                        title: 'Slide 2',
                        width: 1920,
                        height: 1080,
                        layers: [{ id: 'layer-2', name: 'Base', visible: true, objects: [] }]
                    }
                ],
                variables: []
            };
            
            global.fetch = async () => ({
                ok: true,
                text: async () => JSON.stringify(projectJson)
            });
            
            const controller = new PlayerController();
            await controller.initialize('project.json');
            
            // Act
            controller.navigateToSlide('slide-2');
            
            // Assert
            assert.equal(controller.currentSlideId, 'slide-2');
        });
        
        testRunner.it('PlayerController_ButtonClickTriggersNavigation_NavigatesToSlide', async () => {
            // Arrange
            const projectJson = {
                id: 'test-project',
                name: 'Test Project',
                slides: [{
                    id: 'slide-1',
                    title: 'Slide 1',
                    width: 1920,
                    height: 1080,
                    layers: [{
                        id: 'layer-1',
                        name: 'Base',
                        visible: true,
                        objects: [{
                            objectType: 'button',
                            id: 'btn-1',
                            name: 'Next',
                            x: 100,
                            y: 100,
                            width: 200,
                            height: 50,
                            visible: true,
                            label: 'Next',
                            enabled: true,
                            triggers: [{
                                id: 'trigger-1',
                                type: 'OnClick',
                                actions: [{
                                    actionType: 'navigateToSlide',
                                    targetSlideId: 'slide-2'
                                }]
                            }]
                        }]
                    }]
                }, {
                    id: 'slide-2',
                    title: 'Slide 2',
                    width: 1920,
                    height: 1080,
                    layers: [{ id: 'layer-2', name: 'Base', visible: true, objects: [] }]
                }],
                variables: []
            };
            
            global.fetch = async () => ({
                ok: true,
                text: async () => JSON.stringify(projectJson)
            });
            
            const controller = new PlayerController();
            await controller.initialize('project.json');
            controller.renderCurrentSlide();
            
            const buttonObject = projectJson.slides[0].layers[0].objects[0];
            
            // Act
            controller.handleObjectClick(buttonObject);
            
            // Assert
            assert.equal(controller.currentSlideId, 'slide-2');
        });
        
        testRunner.it('ActionExecutor_MultipleActionsInTrigger_ExecutesAll', () => {
            // Arrange
            const variableSystem = new VariableSystem();
            variableSystem.variables.set('var1', { type: 'Number', name: 'Var', value: 0 });
            
            const layerManager = new LayerManager();
            layerManager.layers.set('layer1', { id: 'layer1' });
            
            let navigatedTo = null;
            const playerController = {
                navigateToSlide: (id) => { navigatedTo = id; }
            };
            
            const executor = new ActionExecutor(playerController, variableSystem, layerManager);
            
            const actions = [
                { actionType: 'setVariable', variableId: 'var1', value: 100 },
                { actionType: 'navigateToSlide', targetSlideId: 'slide-2' },
                { actionType: 'showLayer', layerId: 'layer1' }
            ];
            
            // Act
            for (const action of actions) {
                executor.executeAction(action, {});
            }
            
            // Assert
            assert.equal(variableSystem.get('var1'), 100);
            assert.equal(navigatedTo, 'slide-2');
            assert.true(layerManager.isVisible('layer1'));
        });
    });
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = runIntegrationTests;
}