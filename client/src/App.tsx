import React from 'react';
import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import ChooseFamilyNamePage from './pages/ChooseFamilyNamePage';
import FamilyNameProvider from './providers/FamilyNameProvider';
import './App.css';
import FamilyProfilePage from './pages/FamilyProfilePage';
import FamilyElementProvider from './providers/FamilyElementProvider';
import FamilyTreePage from './pages/FamilyTreePage';
import TitleProvider from './providers/TitleProvider';
import FamilySubTreePage from './pages/FamilySubTreePage';

const App: React.FC = () => {
  return (
    <TitleProvider>
      <FamilyNameProvider>
        <FamilyElementProvider>
          <Router>
              <Routes>
                <Route path='/' element={<ChooseFamilyNamePage />}/>
                <Route path='/family-tree' element={<FamilyTreePage/>}/>
                <Route path='/family-profile' element={<FamilyProfilePage/>}/>
                <Route path='/sub-tree' element={<FamilySubTreePage />}/>
              </Routes>
          </Router>
        </FamilyElementProvider>
      </FamilyNameProvider>
    </TitleProvider>
  )
}

export default App;
