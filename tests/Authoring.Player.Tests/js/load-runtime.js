/**
 * Load the runtime.js file for testing
 * This simulates loading the runtime in a Node.js environment
 */

// Mock DOM globals for Node.js
if (typeof global !== 'undefined' && typeof document === 'undefined') {
    // Create minimal DOM simulation
    global.document = {
        createElement: (tag) => {
            const element = {
                tagName: tag.toUpperCase(),
                style: {},
                className: '',
                classList: {
                    add: function(cls) { this.classList = [...(this.classList || []), cls]; },
                    remove: function(cls) { this.classList = this.classList.filter(c => c !== cls); },
                    contains: function(cls) { return (this.classList || []).includes(cls); }
                },
                textContent: '',
                innerHTML: '',
                src: '',
                alt: '',
                type: '',
                disabled: false,
                dataset: {},
                appendChild: function(child) { this.children = [...(this.children || []), child]; return child; },
                querySelector: function(sel) { return null; },
                querySelectorAll: function(sel) { return []; },
                addEventListener: function() {},
                children: [],
                appendChild: function() { return null; }
            };
            return element;
        },
        getElementById: (id) => {
            return {
                id: id,
                style: { display: 'none' },
                textContent: '',
                disabled: false,
                addEventListener: function() {}
            };
        },
        addEventListener: function() {}
    };

    global.window = {
        location: { search: '' },
        addEventListener: function() {},
        URLSearchParams: class {
            constructor(query) { this.query = query || ''; }
            get(name) { return null; }
        }
    };

    global.fetch = async (url) => {
        // Mock fetch - tests will override this
        return {
            ok: true,
            status: 200,
            statusText: 'OK',
            text: async () => '{}',
            json: async () => ({})
        };
    };

    global.performance = {
        now: () => Date.now()
    };

    global.requestAnimationFrame = (cb) => setTimeout(cb, 16);
    global.cancelAnimationFrame = (id) => clearTimeout(id);
}

// Note: In a real environment, you would load runtime.js using fs.readFileSync or similar
// For now, tests will need to copy the relevant classes or use a different approach