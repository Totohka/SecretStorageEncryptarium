import { useEffect, useState } from "react";
import { changeEmail, getMe } from "../../utils/storageClient";
import s from './style.module.css';
import edit from './../../assets/icons/edit.svg';
import Input from "../../components/Input/Input";
import Button from "../../components/Button/Button";
const Profile = () =>{
    const [user, setUser] = useState(null);
    const [show, setShow] = useState(false);
    const [email, setEmail] = useState('');
    const [isActiveInput, setActiveInput] = useState(false);
    async function getMeData() {
        const result = await getMe();
        console.log(result);
        if (result.isSuccess) {
            setUser(result.data);
            setEmail(result.data.email);
            setShow(true);
        }
        else {
            console.log(result.error);
            setShow(false);
        }
    }
    async function editEmail() {
        setActiveInput(false);
        const result = await changeEmail(email);
        console.log(result);
        if (result.isSuccess) {
            if (!result.data){
                alert('Такой email уже используется другим пользователем!');
                setEmail(user.email);
            }
            setShow(true);
        }
        else {
            setEmail(user.email);
            alert('При смене email произошла ошибка!');
            console.log(result.error);
            setShow(true);
        } 
    }
    useEffect(()=>{
        getMeData();
    }, [])

    return(
    <>
        {
            show ? 
            <>
            <div className={s.module}>
                <div className={s.module_container}>
                    <div className={s.module_header}>Uid</div>
                    <div className={s.module_value}>{user.uid}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Логин</div>
                    <div className={s.module_value}>{user.login}</div>
                    {/* <img className={s.module_icon} src={edit}/> */}
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Email</div>
                    {isActiveInput?<input type='email' className={s.input} onChange={(e)=>setEmail(e.target.value)} value={email}/>:<div className={s.module_value}>{email}</div>}
                    {isActiveInput?
                        <>
                            <Button text="Сохранить" classStyle={s.button} onClick={()=>{setShow(false); editEmail()}}/>
                            <Button text="Отменить" classStyle={s.button} onClick={()=>{setEmail(user.email); setActiveInput(false)}}/>
                        </>:
                    <img className={s.module_icon} src={edit} onClick={()=>setActiveInput(true)}/>}
                    
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Дата создания профиля</div>
                    <div className={s.module_value}>{user.dateCreate}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Права администратора</div>
                    <div className={s.module_value}>{user.isAdmin?'Доступны':'Не имеет'}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Права для создания хранилищ</div>
                    <div className={s.module_value}>{user.isCreateStorage?'Доступны':'Не имеет'}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Права для определения групповых ролей</div>
                    <div className={s.module_value}>{user.isCreateGroupRole?'Доступны':'Не имеет'}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Профиль</div>
                    <div className={s.module_value}>{user.isActive?'Активный':'Неактивный'}</div>
                </div>
            </div>   
            </>
            :
            <>Загрузка...</>
        }
    </>
    );
}

export default Profile;