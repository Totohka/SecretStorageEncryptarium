import { useEffect, useState } from "react";
import icon from '../../assets/icons/search.svg';
import s from './style.module.css';
import axios from "axios";
import { useParams } from "react-router";
import Secret from "./Secret/Secret";
import Header from "../../components/Header/Header";
import { createSecret, getSafe, getSecrets } from "../../utils/storageClient";
import Button from "../../components/Button/Button";
import { bodyEditSecret } from "../../entities/bodyEditSecret";
import Popup from "reactjs-popup";
import Input from "../../components/Input/Input";
import { getRightForStorage } from "../../utils/accessClient";
import toast from 'react-hot-toast/headless';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTriangleExclamation, faCircleCheck } from '@fortawesome/free-solid-svg-icons';

const Secrets = () => {
    const [secrets, setSecrets] = useState(undefined);
    const [loading, setLoad] = useState(true);
    const [showed, setShowed] = useState(false);
    const [secret, setSecret] = useState(null);
    const [safe, setSafe] = useState(undefined);
    const [isCreate, setIsCreate] = useState(false);
    const [nameSecret, setNameSecret] = useState("");
    const [valueSecret, setValueSecret] = useState("");
    const param = useParams();
    async function getDataSecrets(){
        const result = await getSecrets({uid:param.uid});
        if (result.isSuccess) setSecrets(result.data);
        else console.log(result.error);
    }
    function handleShow(){
        setShowed(false);
        setSecret(null);
    }
    async function getRight(){
        const result = await getRightForStorage({uid:param.uid});
        if (result.isSuccess) setIsCreate(result.data.isCreate);
        else console.log(result.error);
    }
    async function getDataStorage(){
        const result = await getSafe({uid:param.uid});
        if (result.isSuccess) setSafe(result.data);
        else console.log(result.error);
    }
    async function onCreateSecret(){
        if (nameSecret === "" || valueSecret === ""){
            toast("Пустые поля",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_warning",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
        }
        else{
            bodyEditSecret.name = nameSecret;
            bodyEditSecret.secret = valueSecret;
            bodyEditSecret.storageUid = param.uid;
            const result = await createSecret(bodyEditSecret);
            await getDataSecrets();
            toast("Успешно создано",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_success",
                icon: <FontAwesomeIcon icon={faCircleCheck} style={{color: "#00ff04"}} />,
                removeDelay: 1000
            });
        }
    } 
    useEffect(()=>{
        getDataStorage();
        getDataSecrets();
        getRight();
    },[])
    useEffect(()=>{
        if(secrets!=undefined && safe!=undefined){
            setLoad(false);
        }else setLoad(true);
    },[secrets, safe])
    useEffect(()=>{
        if(secret!=null && secret!=undefined){
            setShowed(false);
            setShowed(true);
        }else setShowed(false);
    },[secret])
    return(
        <>
            {loading?<Header/>:<Header title={safe.title}/>}
            <div className={s.body}>
                <div className={s.container}>
                    <div className={s.header_all}>
                        <div className={s.header}>Секреты</div>
                        { isCreate ?
                            <Popup trigger={
                                <Button classStyle="small" text="Создать"/>
                            }
                            modal>          
                            {close => (
                                <div className={s.popup}>
                                    <p>Создание секрета</p>
                                    <div>
                                        <Input value={nameSecret} alt="Название" type="text" onChange={(e)=>setNameSecret(e.target.value)}/>
                                        <Input value={valueSecret} alt="Значение" type="text" onChange={(e)=>setValueSecret(e.target.value)}/>
                                        <div className={s.popup_content_button}>
                                            <Button classStyle={s.popup_button} text='Создать' onClick={() => {onCreateSecret(); close();}}/>
                                            <Button classStyle={s.popup_button} text='Отмена' onClick={() => {close();}}/>
                                        </div>
                                    </div>
                                </div>
                            )}
                            </Popup>
                            : <></>
                        }
                        
                    </div>
                    <div className={s.search}>
                        <input placeholder="Поиск" type="text" className={s.search_input}/>
                        <img className={s.search_icon} src={icon}/>
                    </div>
                    {loading?<>Загрузка...</>:secrets.length==0?<>Сейф пуст</>:secrets.map((secret)=><button value={secret.uid} className={s.secret_button} onClick={(e)=>{setShowed(false); setSecret(e.target.value)}}>{secret.name}</button>)}
                </div>
                <div className={s.container_info}>
                    {!showed?<></>:<Secret uid={secret} onClose={()=>setSecret(null)} onChange={getDataSecrets} onDelete={handleShow}/>}
                </div>
            </div>
        </>
        
        
    );
}

export default Secrets;