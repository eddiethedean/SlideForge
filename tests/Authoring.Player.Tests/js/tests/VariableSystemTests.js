/**
 * Tests for VariableSystem class
 */

function runVariableSystemTests(testRunner, assert) {
    testRunner.describe('VariableSystem', () => {
        let variableSystem;
        
        // Setup before each test
        variableSystem = new VariableSystem();
        
        testRunner.it('initialize_WithEmptyArray_ClearsVariables', () => {
            // Arrange
            variableSystem.variables.set('test', { value: 100 });
            
            // Act
            variableSystem.initialize([]);
            
            // Assert
            assert.equal(variableSystem.variables.size, 0);
        });
        
        testRunner.it('initialize_WithVariables_InitializesDefaults', () => {
            // Arrange
            const variables = [
                { id: 'var1', name: 'Var 1', type: 'Boolean', defaultValue: true },
                { id: 'var2', name: 'Var 2', type: 'Number', defaultValue: 42 },
                { id: 'var3', name: 'Var 3', type: 'String', defaultValue: 'hello' }
            ];
            
            // Act
            variableSystem.initialize(variables);
            
            // Assert
            assert.equal(variableSystem.get('var1'), true);
            assert.equal(variableSystem.get('var2'), 42);
            assert.equal(variableSystem.get('var3'), 'hello');
        });
        
        testRunner.it('get_WithExistingVariable_ReturnsValue', () => {
            // Arrange
            variableSystem.variables.set('var1', { type: 'Number', name: 'Var', value: 100 });
            
            // Act
            const value = variableSystem.get('var1');
            
            // Assert
            assert.equal(value, 100);
        });
        
        testRunner.it('get_WithNonExistentVariable_ReturnsUndefined', () => {
            // Act
            const value = variableSystem.get('missing');
            
            // Assert
            assert.equal(value, undefined);
        });
        
        testRunner.it('set_WithBooleanVariable_ConvertsToBoolean', () => {
            // Arrange
            variableSystem.variables.set('var1', { type: 'Boolean', name: 'Var', value: false });
            
            // Act
            variableSystem.set('var1', 1);
            
            // Assert
            assert.equal(variableSystem.get('var1'), true);
            assert.equal(typeof variableSystem.get('var1'), 'boolean');
        });
        
        testRunner.it('set_WithNumberVariable_ConvertsToNumber', () => {
            // Arrange
            variableSystem.variables.set('var1', { type: 'Number', name: 'Var', value: 0 });
            
            // Act
            variableSystem.set('var1', '42');
            
            // Assert
            assert.equal(variableSystem.get('var1'), 42);
            assert.equal(typeof variableSystem.get('var1'), 'number');
        });
        
        testRunner.it('set_WithInvalidNumber_DoesNotUpdate', () => {
            // Arrange
            variableSystem.variables.set('var1', { type: 'Number', name: 'Var', value: 100 });
            const originalValue = variableSystem.get('var1');
            
            // Act
            variableSystem.set('var1', 'not-a-number');
            
            // Assert
            assert.equal(variableSystem.get('var1'), originalValue);
        });
        
        testRunner.it('set_WithStringVariable_ConvertsToString', () => {
            // Arrange
            variableSystem.variables.set('var1', { type: 'String', name: 'Var', value: '' });
            
            // Act
            variableSystem.set('var1', 123);
            
            // Assert
            assert.equal(variableSystem.get('var1'), '123');
            assert.equal(typeof variableSystem.get('var1'), 'string');
        });
        
        testRunner.it('set_WithNonExistentVariable_DoesNotThrow', () => {
            // Act & Assert (should not throw)
            variableSystem.set('missing', 100);
            assert.equal(variableSystem.get('missing'), undefined);
        });
        
        testRunner.it('onChange_WithListener_NotifiesOnSet', () => {
            // Arrange
            let notified = false;
            let notifiedVariableId = null;
            let notifiedValue = null;
            
            variableSystem.onChange((variableId, value) => {
                notified = true;
                notifiedVariableId = variableId;
                notifiedValue = value;
            });
            
            variableSystem.variables.set('var1', { type: 'Number', name: 'Var', value: 0 });
            
            // Act
            variableSystem.set('var1', 42);
            
            // Assert
            assert.true(notified);
            assert.equal(notifiedVariableId, 'var1');
            assert.equal(notifiedValue, 42);
        });
        
        testRunner.it('onChange_WithMultipleListeners_NotifiesAll', () => {
            // Arrange
            let count1 = 0;
            let count2 = 0;
            
            variableSystem.onChange(() => count1++);
            variableSystem.onChange(() => count2++);
            
            variableSystem.variables.set('var1', { type: 'Number', name: 'Var', value: 0 });
            
            // Act
            variableSystem.set('var1', 42);
            
            // Assert
            assert.equal(count1, 1);
            assert.equal(count2, 1);
        });
    });
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = runVariableSystemTests;
}