/**
 * Tests for LayerManager class
 */

function runLayerManagerTests(testRunner, assert) {
    testRunner.describe('LayerManager', () => {
        let layerManager;
        
        // Setup before each test
        layerManager = new LayerManager();
        
        testRunner.it('initialize_WithEmptyArray_ClearsLayers', () => {
            // Arrange
            layerManager.layers.set('layer1', { id: 'layer1' });
            layerManager.visibleLayers.add('layer1');
            
            // Act
            layerManager.initialize([]);
            
            // Assert
            assert.equal(layerManager.layers.size, 0);
            assert.equal(layerManager.visibleLayers.size, 0);
        });
        
        testRunner.it('initialize_WithLayers_InitializesCorrectly', () => {
            // Arrange
            const layers = [
                { id: 'layer1', name: 'Layer 1', visible: true, objects: [] },
                { id: 'layer2', name: 'Layer 2', visible: false, objects: [] }
            ];
            
            // Act
            layerManager.initialize(layers);
            
            // Assert
            assert.equal(layerManager.layers.size, 2);
            assert.true(layerManager.isVisible('layer1'));
            assert.false(layerManager.isVisible('layer2'));
        });
        
        testRunner.it('showLayer_WithExistingLayer_MakesVisible', () => {
            // Arrange
            layerManager.layers.set('layer1', { id: 'layer1' });
            
            // Act
            layerManager.showLayer('layer1');
            
            // Assert
            assert.true(layerManager.isVisible('layer1'));
        });
        
        testRunner.it('showLayer_WithNonExistentLayer_DoesNotThrow', () => {
            // Act & Assert (should not throw)
            layerManager.showLayer('missing');
            assert.false(layerManager.isVisible('missing'));
        });
        
        testRunner.it('hideLayer_WithVisibleLayer_MakesHidden', () => {
            // Arrange
            layerManager.layers.set('layer1', { id: 'layer1' });
            layerManager.visibleLayers.add('layer1');
            
            // Act
            layerManager.hideLayer('layer1');
            
            // Assert
            assert.false(layerManager.isVisible('layer1'));
        });
        
        testRunner.it('hideLayer_WithNonExistentLayer_DoesNotThrow', () => {
            // Act & Assert (should not throw)
            layerManager.hideLayer('missing');
        });
        
        testRunner.it('isVisible_WithVisibleLayer_ReturnsTrue', () => {
            // Arrange
            layerManager.visibleLayers.add('layer1');
            
            // Act
            const result = layerManager.isVisible('layer1');
            
            // Assert
            assert.true(result);
        });
        
        testRunner.it('isVisible_WithHiddenLayer_ReturnsFalse', () => {
            // Act
            const result = layerManager.isVisible('layer1');
            
            // Assert
            assert.false(result);
        });
    });
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = runLayerManagerTests;
}