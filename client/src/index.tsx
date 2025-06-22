import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router } from 'react-router-dom';
import './index.css';  // Import global styles for the Family Tree UI
import App from './App';  // Import the main App component which contains our Family Tree UI

// Create the root element for our Family Tree UI React application
const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

// Render the main App component (Family Tree UI) inside the root element
root.render(
  <React.StrictMode>
    <Router>
      <App />
    </Router>
  </React.StrictMode>
);

// Measure and report performance metrics for the Family Tree UI
// Uncomment the line below to enable performance reporting
// reportWebVitals(console.log);

// You might want to send performance data to an analytics service
// Example: reportWebVitals(sendToAnalytics);
// function sendToAnalytics(metric) {
//   // Implement your analytics sending logic here
//   console.log(metric);
// }
