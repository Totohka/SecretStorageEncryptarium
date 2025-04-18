import logo from './logo.svg';
import './App.css';
import Header from './components/Header/Header';
import Footer from './components/Footer/Footer';
import Toolbar from './components/Toolbar/Toolbar';
import Safes from './pages/Safes/Safes';
import { BrowserRouter, Routes, Route } from 'react-router';
import Login from './pages/Login/Login';
import Registration from './pages/Registration/Registration';
import Secrets from './pages/Secrets/Secrets';
import SingleSafe from './pages/Safes/SingleSafe/SingleSafe';
import People from './pages/People/People';
import Rights from './pages/Rights/Rights';
import Profile from './pages/Profile/Profile';
import Notifications from './components/Notifications/Notifications';
import GroupSafes from './pages/GroupSafes/GroupSafes';
import { TypeStorageEnum } from './enums/Enums';
import Admin from './pages/Admin/Admin';

function App() {
  return (
    <BrowserRouter>
      <div className="App">
        <Notifications/>
        <div className='container'>
          <Toolbar/>
          <div className='content'>
            <Routes>
              <Route path='admin' element={<><Header title="Админ панель"/><Admin/></>}/>
              <Route path='profile' element={<><Header title="Мой профиль"/><Profile/></>}/>
              <Route path='safes' element={<><Header title="Мои сейфы"/><Safes code={TypeStorageEnum.None}/></>}/>
              <Route path='organization' element={<><Header title="Сейф организации"/><SingleSafe type='org'/></>}></Route>
              <Route path='personal' element={<><Header title="Личный сейф"/><SingleSafe type='pers'/></>}></Route>
              <Route path='people' element={<><Header title="Коллеги"/><People/></>}></Route>
              <Route path='rights' element={<><Header title="Разрешения"/><Rights/></>}/>
              <Route path='group' element={<><Header title="Групповые сейфы"/><GroupSafes/></>}/>
              <Route path='/registration' element={<Registration/>}/>
              <Route path='' element={<Login/>}/>
              <Route path={`/safe/:uid`} element={<><Secrets/></>}/>
            </Routes>
            <Footer/>
          </div>
        </div>
      </div>
    </BrowserRouter>
    
  );
}

export default App;
