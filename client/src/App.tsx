import React from "react";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import ErrorDisplay from "./components/ErrorDisplay";
import LoadingDisplay from "./components/LoadingDisplay";
import Title from "./components/Title";
import useCriticalAttributes from "./hooks/useCriticalAttributes";
import ChooseFamilyNamePage from "./pages/ChooseFamilyNamePage";
import FamilyTreePage from "./pages/FamilyTreePage";
import FamilyProfilePage from "./pages/FamilyProfilePage";
import FamilySubTreePage from "./pages/FamilySubTreePage";
import { LoadingContext } from "./Enums";
import { isSuccess } from "./Utils";
import './App.css';

// Main App component
const App: React.FC = () => {
  // Hook to fetch critical data for the application
  const {familyNameGetter, familyTreeGetter, selectedPartnershipGetter, titleGetter} = useCriticalAttributes();

  return (
    <>
      {/* Loading and error displays for critical data */}
      <LoadingDisplay context={LoadingContext.RetrieveClientFamilyName} response={familyNameGetter} />
      <ErrorDisplay response={familyNameGetter} />
      <LoadingDisplay context={LoadingContext.RetrieveClientFamilyTree} response={familyTreeGetter} />
      <ErrorDisplay response={familyTreeGetter} />
      <LoadingDisplay context={LoadingContext.RetrieveClientSelectedPartnership} response={selectedPartnershipGetter} />
      <ErrorDisplay response={selectedPartnershipGetter} />
      <LoadingDisplay context={LoadingContext.RetrieveClientTitle} response={titleGetter} />
      <ErrorDisplay response={titleGetter} />

      {/* Main application content - only rendered when all critical data is successfully loaded */}
      {isSuccess(familyNameGetter) && isSuccess(familyTreeGetter) && isSuccess(selectedPartnershipGetter) && isSuccess(titleGetter) && (
        <>
          <Title />
          <Router>
            <Routes>
              <Route path='/' element={<ChooseFamilyNamePage />}/>
              <Route path='/family-tree' element={<FamilyTreePage/>}/>
              <Route path='/family-profile' element={<FamilyProfilePage/>}/>
              <Route path='/sub-tree' element={<FamilySubTreePage />}/>
            </Routes>
          </Router>
        </>
      )}
    </>
  )
}

export default App;
