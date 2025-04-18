import s from './style.module.css';
import button_icon from '../../assets/icons/toolbar_button.svg'
import icon_app from '../../assets/icons/IconApp.svg'
import { useState } from 'react';
import self from "../../assets/icons/self-safe.svg";
import org from "../../assets/icons/org-safe.svg";
import safes from "../../assets/icons/safes.svg";
import perms from "../../assets/icons/permissions.svg";
import collegues from "../../assets/icons/collegues.svg";
import logout from "../../assets/icons/logout.svg";
import profile from "../../assets/icons/profile.svg";
import manage_safe from "../../assets/icons/manage-safe.svg";
import { useNavigate } from 'react-router';
import Cookies from 'js-cookie';

const Toolbar = () => {
    const [showed, setShowed] = useState(false);
    const navigate = useNavigate();
    function logOut(){
        // Cookies.remove("RefreshToken");
        // Cookies.remove("AccessToken");
        navigate('/');
    }
    if (window.location.pathname=="/"||window.location.pathname=="/registration"){
        return(<></>);
    }
    else
    return (
    <div className={showed?s.div:`${s.div} ${s.short_div}`}>
        <div className={showed?s.header:`${s.header} ${s.short}`}>
            <img className={showed?s.icon:`${s.icon} ${s.icon_button}`} src={button_icon} onClick={()=>setShowed(!showed)}/>
            <img className={showed?s.logo:s.hiden} src={icon_app} />
        </div>
        <div className={showed?s.button:`${s.button} ${s.short_button}`} onClick={()=>navigate('/profile')} title='Мой профиль'><img className={showed?`${s.icon_hiden}`:`${s.icon} ${s.icon_button}`} src={profile}/>{showed?'Мой профиль':''}</div>
        <div className={showed?s.button:`${s.button} ${s.short_button}`} onClick={()=>navigate('/personal')} title='Личный сейф'><img className={showed?`${s.icon_hiden}`:`${s.icon} ${s.icon_button}`} src={self}/>{showed?'Личный сейф':''}</div>
        <div className={showed?s.button:`${s.button} ${s.short_button}`} onClick={()=>navigate('/organization')} title='Сейф организации'><img className={showed?`${s.icon_hiden}`:`${s.icon} ${s.icon_button}`} src={org}/>{showed?'Сейф организации':''}</div>
        <div className={showed?s.button:`${s.button} ${s.short_button}`} onClick={()=>navigate('/safes')} title='Сейфы'><img className={showed?`${s.icon_hiden}`:`${s.icon} ${s.icon_button}`} src={safes}/>{showed?'Сейфы':''}</div>
        <div className={showed?s.button:`${s.button} ${s.short_button}`} onClick={()=>navigate('/rights')} title='Разрешения'><img className={showed?`${s.icon_hiden}`:`${s.icon} ${s.icon_button}`} src={perms}/>{showed?'Разрешения':''}</div>
        <div className={showed?s.button:`${s.button} ${s.short_button}`} onClick={()=>navigate('/people')} title='Коллеги'><img className={showed?`${s.icon_hiden}`:`${s.icon} ${s.icon_button}`} src={collegues}/>{showed?'Коллеги':''}</div>
        <div className={showed?s.button:`${s.button} ${s.short_button}`} onClick={()=>navigate('/group')} title='Управление сейфами'><img className={showed?`${s.icon_hiden}`:`${s.icon} ${s.icon_button}`} src={manage_safe}/>{showed?'Управление сейфами':''}</div>
        <div className={showed?`${s.button} ${s.exit}`:`${s.button} ${s.exit} ${s.short_button}`} onClick={logOut}  title='Выйти'><img className={showed?`${s.icon_hiden}`:`${s.icon} ${s.icon_button}`} src={logout}/>{showed?'Выйти':''}</div>
    </div>
    );
}

export default Toolbar;