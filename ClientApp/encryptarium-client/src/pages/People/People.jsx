import { useEffect, useState } from "react";
import { bodyUsers } from "../../entities/bodyUsers";
import { getUsers } from "../../utils/storageClient";
import Human from "./Human/Human";
import s from './style.module.css';
import icon from '../../assets/icons/search.svg';

const People = () => {
    const [email, setEmail] = useState(null);
    const [login, setLogin] = useState(null);
    const [skip, setSkip] = useState(0);
    const [take, setTake] = useState(10);
    const [currentPage, setCurrentPage] = useState(1);
    const [countPage, setCountPage] = useState(1);
    const [users, setUsers] = useState(null);
    const [user, setUser] = useState(null);
    const [loading, setLoad] = useState(true);
    const [showed, setShowed] = useState(false);
    function nextPage(){
        setSkip(skip + 10);
        bodyUsers.skip = skip;
    }
    function prevPage(){
        setSkip(skip - 10);
        bodyUsers.skip = skip;
    }
    async function getData(){
        const result = await getUsers(bodyUsers);
        console.log(result);
        if (result.isSuccess) {
            setUsers(result.data.users);
            setCurrentPage(result.data.currentPage);
            setCountPage(result.data.countPage);
        }
        else console.log(result.error);
    }
    useEffect(()=>{
        getData();
    },[skip])
    useEffect(()=>{
        getData();
    },[])
    useEffect(()=>{
        if(users!==null&&users!=undefined){
            setLoad(false);

        }else setLoad(true);
    },[users])
    useEffect(()=>{
        if(user!=null && user!=undefined){
            setShowed(false);
            setShowed(true);
        }else setShowed(false);
    },[user])
    return (
        <div className={s.body}>
            <div className={s.container}>
                <div className={s.header}>Коллеги</div>
                <div className={s.search}>
                    <input placeholder="Поиск" type="text" className={s.search_input}/>
                    <img className={s.search_icon} src={icon}/>
                </div>
                {loading?<>Загрузка...</>:users.map((user, key)=><button value={key} className={s.user_button} onClick={(e)=>{setShowed(false); setUser(users[e.target.value])}}>{user.login}</button>)}
            </div>
            <div className={s.container_info}>
                {!showed?<></>:<Human human={user}/>}
            </div>
        </div>
    );
}

export default People;