/**
 * Tests for ActionExecutor class
 */

function runActionExecutorTests(testRunner, assert) {
    testRunner.describe('ActionExecutor', () => {
        let actionExecutor;
        let playerController;
        let variableSystem;
        let layerManager;
        
        // Setup before each test
        variableSystem = new VariableSystem();
        layerManager = new LayerManager();
        playerController = {
            navigateToSlide: (slideId) => { playerController.lastNavigateSlideId = slideId; }
        };
        actionExecutor = new ActionExecutor(playerController, variableSystem, layerManager);
        
        testRunner.it('executeAction_WithNavigateToSlide_NavigatesToSlide', () => {
            // Arrange
            const action = {
                actionType: 'navigateToSlide',
                targetSlideId: 'slide-2'
            };
            
            // Act
            actionExecutor.executeAction(action);
            
            // Assert
            assert.equal(playerController.lastNavigateSlideId, 'slide-2');
        });
        
        testRunner.it('executeAction_WithSetVariable_SetsVariable', () => {
            // Arrange
            variableSystem.variables.set('var1', { type: 'Number', name: 'Var', value: 0 });
            const action = {
                actionType: 'setVariable',
                variableId: 'var1',
                value: 42
            };
            
            // Act
            actionExecutor.executeAction(action);
            
            // Assert
            assert.equal(variableSystem.get('var1'), 42);
        });
        
        testRunner.it('executeAction_WithShowLayer_ShowsLayer', () => {
            // Arrange
            layerManager.layers.set('layer1', { id: 'layer1' });
            const action = {
                actionType: 'showLayer',
                layerId: 'layer1'
            };
            const context = {
                slideElement: {
                    querySelector: (sel) => ({
                        classList: { remove: function(cls) {} }
                    })
                }
            };
            
            // Act
            actionExecutor.executeAction(action, context);
            
            // Assert
            assert.true(layerManager.isVisible('layer1'));
        });
        
        testRunner.it('executeAction_WithHideLayer_HidesLayer', () => {
            // Arrange
            layerManager.layers.set('layer1', { id: 'layer1' });
            layerManager.visibleLayers.add('layer1');
            const action = {
                actionType: 'hideLayer',
                layerId: 'layer1'
            };
            const context = {
                slideElement: {
                    querySelector: (sel) => ({
                        classList: { add: function(cls) {} }
                    })
                }
            };
            
            // Act
            actionExecutor.executeAction(action, context);
            
            // Assert
            assert.false(layerManager.isVisible('layer1'));
        });
        
        testRunner.it('executeAction_WithUnknownAction_LogsWarning', () => {
            // Arrange
            const action = {
                actionType: 'unknownAction'
            };
            let warned = false;
            const originalWarn = console.warn;
            console.warn = (msg) => {
                warned = msg.includes('Unknown action type');
            };
            
            // Act
            actionExecutor.executeAction(action);
            
            // Assert
            assert.true(warned);
            
            // Cleanup
            console.warn = originalWarn;
        });
    });
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = runActionExecutorTests;
}