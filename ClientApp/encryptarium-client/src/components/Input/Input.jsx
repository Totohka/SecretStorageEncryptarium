import { EntityEnum, MicroservicesEnum, AuthorizePoliciesEnum, ControllersEnum, PartHttpContextEnum, StatusCodeEnum } from "../../enums/Enums";
import s from "./style.module.css"
import cx from 'classnames';

const Input = ({alt, type, onChange, list, active, value, defaultChecked, width, style}) =>{
    let enumList = null;
    if (list !== null || list !== undefined){
        switch(list){
            case "Microservices":
                enumList = MicroservicesEnum;
                break;
            case "Entities":
                enumList = EntityEnum;
                break;
            case "AuthorizePolicies":
                enumList = AuthorizePoliciesEnum;
                break;
            case "Controllers":
                enumList = ControllersEnum;
                break;
            case "PartHttpContext":
                enumList = PartHttpContextEnum;
                break;
            case "StatusCode":
                enumList = StatusCodeEnum;
                break;
            default:
                break;
        }
    }
    return(
        
            <div className={cx(s.container, s[type])} style={style}>
                <label className={s.label}>{alt}:</label>
                {
                    enumList === null ?
                        <input defaultChecked={defaultChecked} value={value} className={cx(s.input, s[width])} type={type} about={alt} onChange={onChange} disabled={active !== undefined ? !active : false}/>
                    :
                        <select className={s.select} about={alt} list={enumList} onChange={onChange}>
                        {
                            Object.keys(enumList).map((key, index) => ( 
                                <option value={enumList[key]} label={key}>{key}</option> 
                            ))
                        }
                        </select>
                }
            </div>
    );
}
export default Input;