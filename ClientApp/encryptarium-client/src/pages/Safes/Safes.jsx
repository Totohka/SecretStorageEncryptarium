import { useEffect, useState } from "react";
import Safe from "./Safe/Safe";
import icon from '../../assets/icons/search.svg';
import s from './style.module.css';
import { getMe, getSafes } from "../../utils/storageClient";
import { TypeStorageEnum } from "../../enums/Enums";
import Users from "./Users/Users";
import Button from "../../components/Button/Button";

const Safes = ({code}) => {
    const [safes, setSafes] = useState(null);
    const [loading, setLoad] = useState(true);
    const [showed, setShowed] = useState(false);
    const [safe, setSafe] = useState(null);
    const [user, setUser] = useState(null);
    const [loadUser, setLoadUser] = useState(false);
    async function getData(){
        const result = await getSafes(code);
        if (result.isSuccess) setSafes(result.data);
        else console.log(result.error);
    }
    async function getMyUser(){
        const result = await getMe();
        if (result.isSuccess) setUser(result.data);
        else console.log(result.error);
        console.log(result.data);
    }
    function handleShow(){
        setShowed(false);
        setSafe(null);
    }
    const handleSafeClick=(item)=>{
        setShowed(false); 
        if(safe!=item) setSafe(item);
        else setSafe(null);
    }
    useEffect(()=>{
        getData();
        getMyUser();
    },[])
    useEffect(() =>{
        setLoadUser(false);
        if (user !== null) setLoadUser(true);
        else setLoadUser(false);
    }, [user])
    useEffect(()=>{
        if(safes!==null){
            setLoad(false);
        }else setLoad(true);
    },[safes])
    useEffect(()=>{
        if(safe!=null && safe!=undefined){
            setShowed(false);
            setShowed(true);
        }else setShowed(false);
    },[safe])
    return(
        <div className={s.body}>
            <div className={s.container}>
                <div className={s.header}>Все сейфы</div>
                <div className={s.search}>
                    <input placeholder="Поиск" type="text" className={s.search_input}/>
                    <img className={s.search_icon} src={icon}/>
                </div>
                {loading?
                <>Загрузка...</>
                :
                    safes.length == 0 ?
                        <>Пусто</>
                    :
                    safes.map(
                        (item) => 
                            <button value={item.uid} 
                                    className={s.safe_button} 
                                    onClick={()=>handleSafeClick(item.uid)}>
                                        {item.title}
                            </button>
                        )}
            </div>
            {
                code != TypeStorageEnum.Group ?
                <div className={s.container_info}>
                    {!showed?<></>:<Safe uid={safe} onClose={()=>setSafe(null)} onChange={getData} onDelete={handleShow}/>}
                </div>
                :
                <div className={s.container_info}>
                    {!showed?<></>:<Users uid={safe}/>}
                </div>
            }
        </div>
    );
}

export default Safes;