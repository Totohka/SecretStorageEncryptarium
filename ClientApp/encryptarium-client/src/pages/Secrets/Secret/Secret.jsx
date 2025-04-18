import { use, useEffect, useState } from 'react';
import s from './style.module.css';
import axios from 'axios';
import edit from '../../../assets/icons/edit.svg';
import { changeSecret, deleteSecret, getSecret } from '../../../utils/storageClient';
import { getHistory } from '../../../utils/auditClient';
import Input from '../../../components/Input/Input';
import Button from '../../../components/Button/Button';
import History from '../../../components/History/History';
import { bodyHistory } from "../../../entities/bodyHistory";
import { getRightForSecret } from '../../../utils/accessClient';
import Popup from 'reactjs-popup';
import { bodyEditSecret } from '../../../entities/bodyEditSecret';
import toast from 'react-hot-toast/headless';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTriangleExclamation, faCircleCheck } from '@fortawesome/free-solid-svg-icons';
import { EntityEnum, StatusCodeEnum } from '../../../enums/Enums';

const Secret = ({uid, onChange, onDelete}) => {
    const [showsId, setShowsId] = useState([]);
    const [history, setHistory] = useState(null);
    const [secret, setSecret] = useState(null);
    const [userUid, setUserUid] = useState(null);
    const [microservice, setMicroservice] = useState(null); 
    const [controller, setController] = useState(null);
    const [method, setMethod] = useState(null);
    const [authorizePolicies, setAuthorizePolicies] = useState(null);
    const [statusCode, setStatusCode] = useState(null);
    const [dateStart, setDateStart] = useState((new Date(Date.now()-30*60*60*60*24)).toISOString());
    const [dateEnd, setDateEnd] = useState(new Date(Date.now()).toISOString());
    const [entity, setEntity] = useState(null);
    const [entityUid, setEntityUid] = useState(null);
    const [skip, setSkip] = useState(0);
    const [take, setTake] = useState(10);
    const [load, setLoad] = useState(true);
    const [loadHistory, setLoadHistory] = useState(true);
    const [active, setActive] = useState(1);
    const [currentPage, setCurrentPage] = useState(1);
    const [isUpdate, setIsUpdate] = useState(false);
    const [isDelete, setIsDelete] = useState(false);
    const [open, setOpen] = useState(false);
    const [nameSecret, setNameSecret] = useState("");
    const [onValueSecret, setOnValueSecret] = useState(false);
    const [valueSecret, setValueSecret] = useState("");
    axios.defaults.withCredentials = true;

    async function updateSecret({close}){
        if (nameSecret === ""){
            toast("Пустое название",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_warning",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
        }
        else if (nameSecret === "" || valueSecret === "" && onValueSecret){
            toast("Пустое значение",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_warning",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
        }
        else{
            bodyEditSecret.storageUid = secret.storageUid;
            bodyEditSecret.name = nameSecret;
            if (valueSecret === "")
                bodyEditSecret.secret = null;
            else
                bodyEditSecret.secret = valueSecret;
            const result = await changeSecret(secret.uid, bodyEditSecret);
            await getData();
            onChange();
            close();
            toast("Успешно изменено",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_success",
                icon: <FontAwesomeIcon icon={faCircleCheck} style={{color: "#00ff04"}} />,
                removeDelay: 1000
            });
        }
    }

    async function getRight(){
        const result = await getRightForSecret({uid});
        if (result.isSuccess) {
            setIsUpdate(result.data.isUpdate);
            setIsDelete(result.data.isDelete);
        }
    }
    async function getData(){
        const result = await getSecret({uid});
        if (result.isSuccess) {
            setSecret(result.data);
            setNameSecret(result.data.name);
            setValueSecret(result.data.value);
        }
        else console.log(result.error);
    }
    async function getDataHistory(){
        bodyHistory.dateStart = new Date(dateStart);
        bodyHistory.dateEnd = new Date(dateEnd);
        bodyHistory.userUid = userUid;
        bodyHistory.statusCode = Number(statusCode) === StatusCodeEnum.All ? null : Number(statusCode);
        bodyHistory.entity = EntityEnum.Secret;
        bodyHistory.entityUid = uid;
        const result = await getHistory({bodyHistory});
        if (result.isSuccess) {
            setHistory(result.data);
            setLoadHistory(false);
            setCurrentPage(result.data.currentPage);
        }
        else console.log(result.error);
    }
    async function onDeleteSecret(){
        const result = await deleteSecret({uid});
        if (!result.isSuccess) {
            console.log(result.error);
            toast("Непредвиденная ошибка",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_warning",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
        }
        else{
            onChange();
            onDelete();
            toast("Успешно удалено",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_success",
                icon: <FontAwesomeIcon icon={faCircleCheck} style={{color: "#00ff04"}} />,
                removeDelay: 1000
            });
        }
    }
    const handleChangeCheck = () => {
        setOnValueSecret(!onValueSecret);
    };
    function nextPage(){
        bodyHistory.skip += 10;
        setSkip(skip + 10);
    }
    function prevPage(){
        bodyHistory.skip -= 10;
        setSkip(skip - 10);
    }
    useEffect(()=>{
        bodyHistory.userUid = userUid;
        bodyHistory.dateStart = dateStart;
        bodyHistory.dateEnd = dateEnd;
        bodyHistory.skip = skip;
        getDataHistory();
    },[skip])
    
    useEffect(()=>{
        if(secret!==null&&secret!=undefined){
            setLoad(false);
        }else setLoad(true);
    },[secret])
    useEffect(()=>{
        if(history!==null&&history!=undefined){
            setLoadHistory(false);
        }else setLoadHistory(true);
    },[history])
    useEffect(()=>{
        getData();
        getRight();
    },[])
    useEffect(()=>{
        if (active==2){
            bodyHistory.userUid = userUid;
            bodyHistory.dateStart = dateStart;
            bodyHistory.dateEnd = dateEnd;
            bodyHistory.skip = skip;
            getDataHistory();
        }
        
    }, [active])
    return (
        <>
        {load?<>Загрузка...</>:
        <div className={s.container}>
            <div className={s.header}>{secret.name}</div>
            <div className={s.buttons}>
                <button className={active==1?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(1)}}>Общие</button>
                <button className={active==2?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(2)}}>История действий</button>
                <button className={active==3?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(3)}}>Значения</button>
            </div>
            {active==1?
                <div className={s.module}>
                    <div className={s.module_container}>
                        <div className={s.module_header}>Uid</div>
                        <div className={s.module_value}>{secret.uid}</div>
                    </div>
                    <div className={s.module_container}>
                        <div className={s.module_header}>Название</div>
                        <div className={s.module_value}>{secret.name}</div>
                        <img className={s.module_icon} src={edit}/>
                    </div>
                    <div className={s.module_container}>
                        <div className={s.module_header}>Дата создания</div>
                        <div className={s.module_value}>{secret.dateCreate}</div>
                    </div>
                    { isUpdate || isDelete ?
                        <div className={s.module_container_crud}>
                            {
                                isUpdate ?
                                <Popup trigger={
                                    <Button text='Редактировать'/>
                                  }
                                modal>          
                                {close => (
                                    <div className={s.popup}>
                                        <p>Изменение секрета</p>
                                        <div>
                                            <Input value={nameSecret} alt="Название" type="text" onChange={(e) => setNameSecret(e.target.value)}/>
                                            <div className={s.popup_content_value}>
                                                <Input value={valueSecret} active={onValueSecret} alt="Значение" type="text" onChange={(e) => setValueSecret(e.target.value)}/>
                                                <Input alt="Вкл" type="checkbox" onChange={(e) => handleChangeCheck(e.target.check)}/>
                                            </div>
                                            <div className={s.popup_content_button}>
                                                <Button classStyle={s.popup_button} text='Изменить' onClick={() => {updateSecret({close}); }}/>
                                                <Button classStyle={s.popup_button} text='Отмена' onClick={() => {close();}}/>
                                            </div>
                                        </div>
                                    </div>
                                )}
                                </Popup>
                                : <></>
                            }
                            {
                                isDelete ? 
                                <Button text='Удалить' onClick={() => {onDeleteSecret()}}/>
                                : <></>
                            }
                        </div>
                      :
                      <></>
                    }
                    
                </div>
                
            :active==2?
                <div className={s.module}>
                    <div >
                            {loadHistory?<>Загрузка...</>:
                                <div className={s.modele_container_pagination}>
                                <div className={s.module_header}>История</div>
                                    {
                                        skip !== 0 ?
                                        <Button text='<' classStyle="small" onClick={() => {prevPage()}}></Button>
                                        :
                                        <></>
                                    }
                                    <div style={{margin:'0px 10px'}}>{currentPage}</div>
                                    { 
                                        currentPage !== history.count ? 
                                        <Button text='>' classStyle="small" onClick={() => {nextPage()}}></Button>
                                        :
                                        <></>
                                    }
                                    <div className={s.module_value}>
                                        <label>Всего страниц:</label>
                                        {history.count}
                                    </div>
                                </div>
                            }
                        <div className={s.module_container_filter}>
                            <div style={{display: "flex"}}>
                                <Input alt="UserUid" type="text" onChange={(e) => {setUserUid(e.target.value)}}/>
                                <Input alt="StatusCode" type="text" list="StatusCode" onChange={(e) => {setStatusCode(e.target.value)}}/>
                                <Input alt="DateStart" type="datetime-local" onChange={(e) => {setDateStart(e.target.value)}}/>
                                <Input alt="DateEnd" type="datetime-local" onChange={(e) => {setDateEnd(e.target.value)}}/>
                            </div>
                            <Button text="Поиск" onClick={()=>{setLoadHistory(true); getDataHistory();}}/>
                        </div>
                        <div style={{'overflow-y':'auto','height':'51vh'}}>
                            {loadHistory?<>Загрузка...</>:history.audits.map((h, index)=>{
                                return(
                                    <History type={'safe'} h={h} id={index}/>
                                );
                            })}
                        </div>
                    </div>
                </div>
            :
                <div className={s.module}>
                    <div className={s.module_container}>
                        <div className={s.module_header}>Название</div>
                        <div className={s.module_value}>{secret.name}</div>
                        <img className={s.module_icon} src={edit}/>
                    </div>
                    <div className={s.module_container}>
                        <div className={s.module_header}>Значение</div>
                        <div className={s.module_value}>{secret.value}</div>
                        <img className={s.module_icon} src={edit}/>
                    </div>
                </div>
            }                  
        </div>
        }</>
    );
}

export default Secret;