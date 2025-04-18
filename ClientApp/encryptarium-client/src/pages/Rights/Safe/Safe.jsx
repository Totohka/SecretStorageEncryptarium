import { useEffect, useState } from "react";
import { getSafe } from "../../../utils/storageClient";
import Right from "../Right/Right";
import s from './style.module.css';

const Safe = ({uid, value}) => {
    const [safe, setSafe] = useState(null);
    const [show, setShow] = useState(false);
    const [showRight, setShowRight] = useState(false);
    const [right, setRight] = useState(null);

    async function getDataSafe() {
        console.log({uid});
        const result = await getSafe({uid});
        if (result.isSuccess) setSafe(result.data);
        else console.log(result.error);
    }

    useEffect(()=>{
        setRight(value);
        getDataSafe();
    },[])
    useEffect(()=>{
        if (safe !== null && safe !== undefined){
            setShow(true);
        }
        else setShow(false);
    },[safe])
    useEffect(()=>{
        if (right !== null && right !== undefined){
            setShowRight(true);
        } else setShowRight(false);
    }, [right])
    return (
    <div className={s.module_container}>
            {   
                !show?<></>
                :
                <div className={s.box}>
                    <p>Uid: {safe.uid}</p>
                    <p>Название: {safe.title}</p>
                    <p>Описание: {safe.description}</p>
                </div>
            }
            { 
                showRight? right.map((item)=>
                    <Right value={item}></Right>
                )
                :
                <>Загрузка...</>
            }
    </div>
    );
}

export default Safe;