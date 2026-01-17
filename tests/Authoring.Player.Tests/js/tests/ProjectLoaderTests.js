/**
 * Tests for ProjectLoader class
 */

function runProjectLoaderTests(testRunner, assert) {
    // Import runtime classes (in real scenario, these would be loaded from runtime.js)
    // For testing, we'll define them inline or use a loader
    
    // Mock fetch for testing
    const originalFetch = typeof fetch !== 'undefined' ? fetch : null;
    
    testRunner.describe('ProjectLoader', () => {
        let loader;
        
        // Setup before each test
        loader = new ProjectLoader();
        
        testRunner.it('loadProject_WithValidJSON_ReturnsParsedProject', async () => {
            // Arrange
            const mockProject = {
                id: 'test-project',
                name: 'Test Project',
                slides: []
            };
            
            global.fetch = async () => ({
                ok: true,
                status: 200,
                statusText: 'OK',
                text: async () => JSON.stringify(mockProject)
            });
            
            // Act
            const project = await loader.loadProject('test.json');
            
            // Assert
            assert.ok(project);
            assert.equal(project.id, 'test-project');
            assert.equal(project.name, 'Test Project');
        });
        
        testRunner.it('loadProject_WithInvalidJSON_ThrowsError', async () => {
            // Arrange
            global.fetch = async () => ({
                ok: true,
                status: 200,
                text: async () => '{ invalid json }'
            });
            
            // Act & Assert
            let error = null;
            try {
                await loader.loadProject('test.json');
            } catch (e) {
                error = e;
            }
            assert.ok(error);
            assert.includes(error.message, 'Error loading project');
        });
        
        testRunner.it('loadProject_WithHTTPError_ThrowsError', async () => {
            // Arrange
            global.fetch = async () => ({
                ok: false,
                status: 404,
                statusText: 'Not Found',
                text: async () => ''
            });
            
            // Act & Assert
            let error = null;
            try {
                await loader.loadProject('missing.json');
            } catch (e) {
                error = e;
            }
            assert.ok(error);
            assert.includes(error.message, 'Failed to load project');
        });
        
        testRunner.it('loadProject_WithMissingId_ThrowsError', async () => {
            // Arrange
            const invalidProject = {
                name: 'Test Project',
                slides: []
            };
            
            global.fetch = async () => ({
                ok: true,
                text: async () => JSON.stringify(invalidProject)
            });
            
            // Act & Assert
            let error = null;
            try {
                await loader.loadProject('test.json');
            } catch (e) {
                error = e;
            }
            assert.ok(error);
            assert.includes(error.message, 'Invalid project structure');
        });
        
        testRunner.it('loadProject_WithMissingSlides_ThrowsError', async () => {
            // Arrange
            const invalidProject = {
                id: 'test-project',
                name: 'Test Project'
            };
            
            global.fetch = async () => ({
                ok: true,
                text: async () => JSON.stringify(invalidProject)
            });
            
            // Act & Assert
            let error = null;
            try {
                await loader.loadProject('test.json');
            } catch (e) {
                error = e;
            }
            assert.ok(error);
            assert.includes(error.message, 'Invalid project structure');
        });
    });
    
    // Restore original fetch
    if (originalFetch) {
        global.fetch = originalFetch;
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = runProjectLoaderTests;
}