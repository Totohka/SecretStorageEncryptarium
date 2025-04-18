import { useEffect, useState } from 'react';
import './Safe.css'
import s from './style.module.css';
import edit from '../../../assets/icons/edit.svg'
import { NavLink } from 'react-router';
import { changeSafe, deleteSafe, getSafe } from '../../../utils/storageClient';
import Button from '../../../components/Button/Button';
import Input from '../../../components/Input/Input';
import History from '../../../components/History/History';
import { getHistory } from '../../../utils/auditClient';
import { bodyHistory } from "../../../entities/bodyHistory";
import { getRightForStorage } from '../../../utils/accessClient';
import toast from 'react-hot-toast/headless';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCircleCheck, faTriangleExclamation } from '@fortawesome/free-solid-svg-icons';
import { bodyEditSafe } from '../../../entities/bodyEditSafe';
import Popup from 'reactjs-popup';
import { EntityEnum, StatusCodeEnum } from '../../../enums/Enums';

const Safe = ({uid, onChange, onDelete}) => {
    const [safe, setSafe] = useState(null);
    const [history, setHistory] = useState(null);
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
    const [load, setLoad] = useState(true);
    const [active, setActive] = useState(1);    
    const [skip, setSkip] = useState(0);
    const [take, setTake] = useState(10);
    const [currentPage, setCurrentPage] = useState(1);
    const [loadHistory, setLoadHistory] = useState(true);
    const [isUpdate, setIsUpdate] = useState(false);
    const [isDelete, setIsDelete] = useState(false);
    const [nameSafe, setNameSafe] = useState("");
    const [descriptionSafe, setDescriptionSafe] = useState("");
    async function updateSafe({close}){
        if (nameSafe === "" || descriptionSafe === ""){
            toast("Пустое название или описание",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_warning",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
        }
        else{
            bodyEditSafe.title = nameSafe;
            bodyEditSafe.description = descriptionSafe;
            const result = await changeSafe(safe.uid, bodyEditSafe);
            if (result.isSuccess){
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
            else{
                close();
                toast("Произошла непредвиденная ошибка",{
                    duration: 4000,
                    position: 'bottom-right',
                    className: "notification_success",
                    icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                    removeDelay: 1000
                });
            }
        }
    }
    async function delSafe({close}){
        const result = await deleteSafe(safe.uid);
        if (result.isSuccess){
            close();
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
        else{
            toast("Произошла непредвиденная ошибка",{
                duration: 4000,
                position: 'bottom-right',
                className: "notification_success",
                icon: <FontAwesomeIcon icon={faTriangleExclamation} style={{color: "#ff0000"}} />,
                removeDelay: 1000
            });
        }
    }
    async function getDataHistory(){
        bodyHistory.authorizePolicies = null;
        bodyHistory.controller = null;
        bodyHistory.method = null;
        bodyHistory.microservice = null;
        bodyHistory.entity = EntityEnum.Storage;
        bodyHistory.entityUid = uid;
        bodyHistory.dateStart = new Date(dateStart);
        bodyHistory.dateEnd = new Date(dateEnd);
        bodyHistory.statusCode = Number(statusCode) === StatusCodeEnum.All ? null : Number(statusCode)
        bodyHistory.userUid = userUid;
        const result = await getHistory({bodyHistory});
        if (result.isSuccess) {
            setHistory(result.data);
            setLoadHistory(false);
            setCurrentPage(result.data.currentPage);
        }
        else console.log(result.error);
    }
    async function getRight(){
        const result = await getRightForStorage({uid});
        if (result.isSuccess) {
            setIsUpdate(result.data.isUpdate);
            setIsDelete(result.data.isDelete);
        }
    }
    function nextPage(){
        bodyHistory.skip += 10;
        setSkip(skip + 10);
    }
    function prevPage(){
        bodyHistory.skip -= 10;
        setSkip(skip - 10);
    }
    async function getData(){
        const result = await getSafe({uid});
        if (result.isSuccess) setSafe(result.data);
        else console.log(result.error);
    }
    useEffect(()=>{
        bodyHistory.userUid = userUid;
        bodyHistory.dateStart = dateStart;
        bodyHistory.dateEnd = dateEnd;
        bodyHistory.skip = skip;
        getDataHistory();
    },[skip])

    useEffect(()=>{
        if(safe!==null&&safe!=undefined){
            setLoad(false);
        }else setLoad(true);
    },[safe])
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
    useEffect(()=>{
        if(history!==null&&history!=undefined){
            setLoadHistory(false);
        }else setLoadHistory(true);
    },[history])
    return (
        <>
        {load?<>Загрузка...</>:
        <div className={s.container}>
            <div className={s.header}>{safe.title}</div>
            <div className={s.buttons}>
                <button className={active==1?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(1)}}>Общие</button>
                <button className={active==2?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(2)}}>История действий</button>
            </div>
            {active==1?
            <div className={s.module}>
                <div className={s.module_container}>
                    <div className={s.module_header}>Uid</div>
                    <div className={s.module_value}>{safe.uid}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Название</div>
                    <div className={s.module_value}>{safe.title}</div>
                    <img className={s.module_icon} src={edit}/>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Описание</div>
                    <div className={s.module_value}>{safe.description}</div>
                    <img className={s.module_icon} src={edit}/>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Дата создания</div>
                    <div className={s.module_value}>{safe.dateCreate}</div>
                </div>
                <div className={s.module_container}>
                    <div className={s.module_header}>Статус</div>
                    <div className={s.module_value}>{safe.isCommon?'Общедоступный':'Закрытый'}</div>
                </div>
                { isDelete || isUpdate ?
                    <div className={s.module_container}>
                        <div className={s.module_header}>Управление</div>
                        <div className={s.module_value_manage}>
                            {   isUpdate ?
                                <>
                                <Popup trigger={
                                    <Button text='Редактировать' classStyle="small"/>
                                    }
                                modal>          
                                {close => (
                                    <div className={s.popup}>
                                        <p>Изменение сейфа</p>
                                        <div>
                                            <Input value={nameSafe} alt="Название" type="text" onChange={(e) => setNameSafe(e.target.value)}/>
                                            <Input value={descriptionSafe} alt="Описание" type="text" onChange={(e) => setDescriptionSafe(e.target.value)}/>
                                            <div className={s.popup_content_button}>
                                                <Button classStyle={s.popup_button} text='Изменить' onClick={() => {updateSafe({close}); }}/>
                                                <Button classStyle={s.popup_button} text='Отмена' onClick={() => {close();}}/>
                                            </div>
                                        </div>
                                    </div>
                                )}
                                </Popup>
                                <div style={{margin: '0px 10px'}}></div>
                                </>
                                : <></>
                            }
                            {   isDelete ?
                                <Popup trigger={
                                    <Button text='Удалить' classStyle="small"/>
                                    }
                                modal>          
                                {close => (
                                    <div className={s.popup}>
                                        <p>Удаление сейфа</p>
                                        <div>
                                            <div className={s.popup_content_button}>
                                                <Button classStyle={s.popup_button} text='Удалить' onClick={() => {delSafe({close}); }}/>
                                                <Button classStyle={s.popup_button} text='Отмена' onClick={() => {close();}}/>
                                            </div>
                                        </div>
                                    </div>
                                )}
                                </Popup>
                                : <></>
                            }
                        </div>
                    </div>
                    : <></>
                }
                <NavLink className={s.link} to={`/safe/${uid}`}>Открыть сейф</NavLink>
            </div>   
            : 
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
                                <Input alt="UserUid" type="text" onChange={(e)=>setUserUid(e.target.value)}/>
                                <Input alt="StatusCode" type="text" list="StatusCode" onChange={(e)=>setStatusCode(e.target.value)}/>
                                <Input alt="DateStart" type="datetime-local" onChange={(e)=>setDateStart(e.target.value)}/>
                                <Input alt="DateEnd" type="datetime-local" onChange={(e)=>setDateEnd(e.target.value)}/>
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
            }
        </div>
        }
        </>
    );
}

export default Safe;