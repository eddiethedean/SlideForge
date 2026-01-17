/**
 * Simple Test Runner for SlideForge Player Runtime Tests
 * Run with: node test-runner.js
 */

// Simple test framework
class TestRunner {
    constructor() {
        this.tests = [];
        this.passed = 0;
        this.failed = 0;
        this.currentSuite = null;
    }

    describe(suiteName, fn) {
        this.currentSuite = suiteName;
        console.log(`\n${suiteName}`);
        console.log('='.repeat(suiteName.length));
        fn();
        this.currentSuite = null;
    }

    it(testName, fn) {
        try {
            fn();
            this.passed++;
            console.log(`  ✓ ${testName}`);
        } catch (error) {
            this.failed++;
            console.log(`  ✗ ${testName}`);
            console.log(`    Error: ${error.message}`);
            if (error.stack) {
                console.log(`    ${error.stack.split('\n')[1]}`);
            }
        }
    }

    summary() {
        console.log('\n' + '='.repeat(50));
        console.log(`Tests: ${this.passed + this.failed} | Passed: ${this.passed} | Failed: ${this.failed}`);
        console.log('='.repeat(50));
        return this.failed === 0;
    }
}

// Assertions
const assert = {
    equal(actual, expected, message) {
        if (actual !== expected) {
            throw new Error(message || `Expected ${expected}, got ${actual}`);
        }
    },
    notEqual(actual, expected, message) {
        if (actual === expected) {
            throw new Error(message || `Expected not ${expected}, got ${actual}`);
        }
    },
    true(actual, message) {
        if (actual !== true) {
            throw new Error(message || `Expected true, got ${actual}`);
        }
    },
    false(actual, message) {
        if (actual !== false) {
            throw new Error(message || `Expected false, got ${actual}`);
        }
    },
    ok(actual, message) {
        if (!actual) {
            throw new Error(message || `Expected truthy value, got ${actual}`);
        }
    },
    throws(fn, message) {
        try {
            fn();
            throw new Error(message || 'Expected function to throw');
        } catch (error) {
            // Expected to throw
            return error;
        }
    },
    includes(collection, item, message) {
        if (!collection.includes(item)) {
            throw new Error(message || `Expected ${collection} to include ${item}`);
        }
    },
    instanceOf(actual, expected, message) {
        if (!(actual instanceof expected)) {
            throw new Error(message || `Expected instance of ${expected.name}, got ${typeof actual}`);
        }
    },
    deepEqual(actual, expected, message) {
        if (JSON.stringify(actual) !== JSON.stringify(expected)) {
            throw new Error(message || `Expected ${JSON.stringify(expected)}, got ${JSON.stringify(actual)}`);
        }
    }
};

// Export for Node.js
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { TestRunner, assert };
}

// Global for browser
if (typeof window !== 'undefined') {
    window.TestRunner = TestRunner;
    window.assert = assert;
}