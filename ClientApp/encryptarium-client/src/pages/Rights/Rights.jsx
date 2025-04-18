import { useEffect, useState } from 'react';
import s from './style.module.css';
import { getRightSecret, getRightStorage } from '../../utils/accessClient';
import Safe from './Safe/Safe';

const Rights = () => {
    const [active, setActive] = useState(2);
    const [rightStorage, setRightStorage] = useState(null);
    const [rightSecret, setRightSecret] = useState(null);
    const [showStorage, setShowStorage] = useState(false);
    const [showSecret, setShowSecret] = useState(false);
    async function getDataForStorage(){
        const result = await getRightStorage();
        if (result.isSuccess) {
            setRightStorage(result.data.accessModel);
            console.log(result.data.accessModel);
        }
        else console.log(result.error);
    }
    async function getDataForSecret(){
        const result = await getRightSecret();
        if (result.isSuccess) {
            setRightSecret(result.data.accessModel);
            console.log(result.data.accessModel);
        }
        else console.log(result.error);
    }

    useEffect(()=>
    {
        if (active == 1){
            getDataForSecret();
        }
        else{
            getDataForStorage();
        }
    } ,[active])

    useEffect(()=>
        {
            if (rightSecret !== undefined && rightSecret !== null){
                setShowSecret(true);
            }
            else setShowSecret(false);
        }, [rightSecret]
    )

    useEffect(()=>
        {
            if (rightStorage !== undefined && rightStorage !== null){
                setShowStorage(true);
            }
            else setShowStorage(false);
        }, [rightStorage]
    )

    return (
        <div className={s.body}>
            <div className={s.container_info}>
                <div className={s.container}>
                    <div className={s.buttons}>
                        <button className={active==1?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(1)}}>Секреты</button>
                        <button className={active==2?`${s.button} ${s.active}`:s.button} onClick={()=>{setActive(2)}}>Хранилища</button>
                    </div>
                    {active==1?
                        <div>
                        {
                            !showSecret? <></>
                            :
                            <div>
                            {
                                Object.keys(rightSecret).map((key, index) => ( 
                                    <p key={index}> this is my key {key} and this is my value {String(rightSecret[`${key}`])}</p> 
                                  ))
                            }
                            </div>
                        }
                        </div>
                        :
                        <div>
                            {
                            !showStorage? <></>
                            :
                            <div>
                            {
                                Object.keys(rightStorage).map((key, index) => {
                                    // console.log(rightStorage[key])
                                    return <Safe uid={key} value={rightStorage[key]}/>
                                })
                            }
                            </div>
                        }
                        </div>
                    }
                </div>
            </div>
        </div>
    );
}

export default Rights;