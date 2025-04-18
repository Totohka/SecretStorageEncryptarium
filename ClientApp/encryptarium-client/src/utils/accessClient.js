import axios from "axios";
import { toRefresh } from "./authClient";

axios.defaults.withCredentials = true;

export const accessClient = axios.create({
    baseURL:"https://localhost:7061/api/v1",
    withCredentials: true,
    headers: "Access-Control-Allow-Origin"
})

accessClient.interceptors.response.use(async(response) => {
    if(response.status===401){
        console.log(response);
    }
    return response;
}
, async (error) => {
    console.log('interceptor');
    console.log(error.response);
    console.log(error.toJSON());
    if (error.response.status === 401) {
        const result = await toRefresh();
        if (!result.isSuccess){
            window.location.href = '/';
        }
        return axios.request(error.config);
    }
    return Promise.reject(error);
}
);
export const changeAccessGroupStorage = async(storageUid, data) => {
    try{
        const response = await accessClient.put(`/UserRight/ChangeAccessForGroupStorage?storageUid=${storageUid}`, data, {withCredentials: true});
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const createGroupRoleAndStorage = async(data) => {
    try{
        const response = await accessClient.post("/Role/CreateGroupRoleAndStorage", data, {withCredentials: true});
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const getRightStorage = async() => {
    axios.defaults.withCredentials = true;
    try{
        const response = await accessClient.get("/UserRight/GetRightStorage", {

            withCredentials: true
        });
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}

export const getRightForSecret = async({uid}) => {
    axios.defaults.withCredentials = true;
    try{
        const response = await accessClient.get(`/UserRight/GetRightForSecret?secretUid=${uid}`, {

            withCredentials: true
        });
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}

export const getRightForStorage = async({uid}) => {
    axios.defaults.withCredentials = true;
    try{
        const response = await accessClient.get(`/UserRight/GetRightForStorage?storageUid=${uid}`, {

            withCredentials: true
        });
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}

export const getRightSecret = async() => {
    axios.defaults.withCredentials = true;
    try{
        const response = await accessClient.get("/UserRight/GetRightStorage", {

            withCredentials: true
        });
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}