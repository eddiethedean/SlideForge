/**
 * Main test runner for SlideForge Player Runtime Tests
 * 
 * Usage:
 * - Browser: Open this file in a browser (with runtime.js loaded first)
 * - Node.js: node all-tests.js (requires runtime.js to be accessible)
 */

// Load test runner
let TestRunner, assert;
if (typeof module !== 'undefined' && module.exports) {
    // Node.js
    const testFramework = require('./test-runner.js');
    TestRunner = testFramework.TestRunner;
    assert = testFramework.assert;
} else {
    // Browser
    TestRunner = window.TestRunner;
    assert = window.assert;
}

// Load runtime classes (assumes runtime.js is loaded)
// In browser: <script src="../../src/Authoring.Player/www/js/runtime.js"></script>
// In Node.js: require or load runtime.js

// Load test suites
let runProjectLoaderTests, runVariableSystemTests, runLayerManagerTests;
let runTimelineEngineTests, runActionExecutorTests, runTriggerEvaluatorTests;
let runIntegrationTests;

if (typeof module !== 'undefined' && module.exports) {
    // Node.js - load from files
    runProjectLoaderTests = require('./tests/ProjectLoaderTests.js');
    runVariableSystemTests = require('./tests/VariableSystemTests.js');
    runLayerManagerTests = require('./tests/LayerManagerTests.js');
    runTimelineEngineTests = require('./tests/TimelineEngineTests.js');
    runActionExecutorTests = require('./tests/ActionExecutorTests.js');
    runTriggerEvaluatorTests = require('./tests/TriggerEvaluatorTests.js');
    runIntegrationTests = require('./tests/IntegrationTests.js');
}

function runAllTests() {
    const runner = new TestRunner();
    
    console.log('SlideForge Player Runtime Tests');
    console.log('================================\n');
    
    // Run test suites (only if runtime classes are available)
    if (typeof ProjectLoader !== 'undefined') {
        runProjectLoaderTests(runner, assert);
    }
    
    if (typeof VariableSystem !== 'undefined') {
        runVariableSystemTests(runner, assert);
    }
    
    if (typeof LayerManager !== 'undefined') {
        runLayerManagerTests(runner, assert);
    }
    
    if (typeof TimelineEngine !== 'undefined') {
        runTimelineEngineTests(runner, assert);
    }
    
    if (typeof ActionExecutor !== 'undefined') {
        runActionExecutorTests(runner, assert);
    }
    
    if (typeof TriggerEvaluator !== 'undefined') {
        runTriggerEvaluatorTests(runner, assert);
    }
    
    if (typeof PlayerController !== 'undefined') {
        runIntegrationTests(runner, assert);
    }
    
    // Print summary
    const success = runner.summary();
    process.exit(success ? 0 : 1);
}

// Run tests if this is the main module
if (typeof module !== 'undefined' && require.main === module) {
    // Try to load runtime.js
    try {
        // In Node.js, we'd need to evaluate the runtime.js file
        // This is a simplified version - in practice, use a tool like jsdom or headless browser
        console.log('Note: These tests require the runtime classes to be loaded.');
        console.log('For full testing, use a browser-based test runner or Puppeteer/Playwright.\n');
    } catch (error) {
        console.error('Error loading runtime:', error.message);
    }
    
    runAllTests();
}

// Export for browser use
if (typeof window !== 'undefined') {
    window.runAllTests = runAllTests;
}