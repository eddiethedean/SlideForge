# SlideForge Player Runtime Tests

JavaScript unit and integration tests for the SlideForge HTML/JavaScript Runtime Player.

## Test Structure

- `test-runner.js` - Simple test framework
- `tests/` - Individual test suites
  - `ProjectLoaderTests.js` - Tests for ProjectLoader class
  - `VariableSystemTests.js` - Tests for VariableSystem class
  - `LayerManagerTests.js` - Tests for LayerManager class
  - `TimelineEngineTests.js` - Tests for TimelineEngine class
  - `ActionExecutorTests.js` - Tests for ActionExecutor class
  - `TriggerEvaluatorTests.js` - Tests for TriggerEvaluator class
  - `IntegrationTests.js` - Integration tests for PlayerController
- `all-tests.js` - Main test runner

## Running Tests

### Option 1: Browser-based Testing (Recommended)

Create an HTML test page:

```html
<!DOCTYPE html>
<html>
<head>
    <title>Player Runtime Tests</title>
</head>
<body>
    <script src="../../../src/Authoring.Player/www/js/runtime.js"></script>
    <script src="test-runner.js"></script>
    <script src="tests/ProjectLoaderTests.js"></script>
    <script src="tests/VariableSystemTests.js"></script>
    <script src="tests/LayerManagerTests.js"></script>
    <script src="tests/TimelineEngineTests.js"></script>
    <script src="tests/ActionExecutorTests.js"></script>
    <script src="tests/TriggerEvaluatorTests.js"></script>
    <script src="tests/IntegrationTests.js"></script>
    <script src="all-tests.js"></script>
    <script>runAllTests();</script>
</body>
</html>
```

Open the HTML file in a browser to see test results.

### Option 2: Node.js with Puppeteer/Playwright

For automated testing, use Puppeteer or Playwright to run tests in a headless browser:

```bash
npm install puppeteer
node test-with-puppeteer.js
```

### Option 3: Manual Testing

Use the manual testing checklist in `../../PlayerTesting.md`.

## Test Coverage

### ProjectLoader Tests
- Load valid project JSON
- Handle invalid JSON
- Handle HTTP errors
- Validate project structure

### VariableSystem Tests
- Initialize variables from project defaults
- Get variable values
- Set variable values with type validation
- Type conversion (Boolean, Number, String)
- Variable change notifications

### LayerManager Tests
- Initialize layers
- Show/hide layers
- Check layer visibility

### TimelineEngine Tests
- Play/pause/reset timeline
- Object visibility based on timeline
- Timeline start callbacks

### ActionExecutor Tests
- NavigateToSlide action
- SetVariable action
- ShowLayer/HideLayer actions
- Unknown action handling

### TriggerEvaluator Tests
- OnClick trigger evaluation
- OnTimelineStart trigger evaluation
- Non-matching trigger types

### Integration Tests
- Full player initialization
- Slide navigation
- Button click triggers
- Multiple actions in sequence

## Adding New Tests

1. Create a new test file in `tests/` directory
2. Follow the pattern:
   ```javascript
   function runMyTests(testRunner, assert) {
       testRunner.describe('MyClass', () => {
           testRunner.it('testName_Scenario_ExpectedBehavior', () => {
               // Arrange
               // Act
               // Assert
           });
       });
   }
   ```
3. Load the test file in `all-tests.js`
4. Call the test function in `runAllTests()`

## Notes

- Tests use a simple custom test framework (no external dependencies)
- Tests can run in browser or Node.js (with appropriate runtime loading)
- For production use, consider integrating with Jest, Mocha, or similar frameworks
- DOM mocking is minimal - full integration tests should use real browser environment