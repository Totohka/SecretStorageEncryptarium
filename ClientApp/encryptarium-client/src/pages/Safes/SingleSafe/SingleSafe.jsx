import { useEffect, useState } from "react";
import { getOrganizationSafe, getPersonSafe, getSafe } from "../../../utils/storageClient";
import Safe from "../Safe/Safe";
import s from './style.module.css';

const SingleSafe = ({type}) => {
    const [showed, setShowed] = useState(false);
    const [safe, setSafe] = useState(null);
    async function getData(){
        if (type === 'org'){
            setShowed(false);
            const result = await getOrganizationSafe();
            if (result.isSuccess) {
                const safeResult = await getSafe({uid: result.data});
                setSafe(safeResult.data);
            }
            else console.log(result.error);
        }
        else if (type === 'pers'){
            setShowed(false);
            const result = await getPersonSafe();
            if (result.isSuccess) {
                const safeResult = await getSafe({uid: result.data});
                setSafe(safeResult.data);
            }
            else console.log(result.error);
        }
    }
    useEffect(()=>{
        setShowed(false);
        getData();
    },[])
    useEffect(()=>{
        setShowed(false);
        getData();
    },[type])
    useEffect(()=>{
        if(safe!=null && safe!=undefined){
            setShowed(false);
            setShowed(true);
        }else setShowed(false);
    },[safe])
    return(
        <div className={s.body}>
            <div className={s.container_info}>
                {!showed?<></>:<Safe uid={safe.uid} onClose={()=>setSafe(null)}/>}
            </div>
        </div>
    );
}

export default SingleSafe;