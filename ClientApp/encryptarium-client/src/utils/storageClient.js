import axios from "axios";
import { toRefresh } from "./authClient";
import { data, useNavigate } from "react-router";
import { bodyUsers } from "../entities/bodyUsers";
import { bodyEditSecret } from "../entities/bodyEditSecret";
axios.defaults.withCredentials = true;
const storageClient = axios.create({
    baseURL:"https://localhost:7069/api/v1",
    withCredentials: true,
    headers: "Access-Control-Allow-Origin"
})

storageClient.interceptors.response.use(async(response) => {
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

export const getUser = async(uid) => {
    try{
        const response = await storageClient.get(`/User/${uid}`, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}

export const getUsersByStorageUid = async(uid) =>{
    try{
        const response = await storageClient.get(`/User/GetUsersByStorage?storageUid=${uid}`, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}

export const getMe = async() => {
    try{
        const response = await storageClient.get("/User/getme", {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}

export const createSecret = async(bodyEditSecret) => {
    try{
        const response = await storageClient.post("/Secret", bodyEditSecret, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const deleteSecret = async({uid}) => {
    try{
        const response = await storageClient.delete(`/Secret/${uid}`, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const changeSecret = async(uid, bodyEditSecret) => {
    try{
        const response = await storageClient.put(`/Secret/${uid}`, bodyEditSecret, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const deleteSafe = async(uid) => {
    try{
        const response = await storageClient.delete(`/Storage/${uid}`, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const changeSafe = async(uid, bodyEditSafe) => {
    try{
        const response = await storageClient.put(`/Storage/${uid}`, bodyEditSafe, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const changeEmail = async(email) => {
    try{
        const response = await storageClient.get(`/User/change/email/${email}`, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const getAllForAdmin = async(bodyUsers) => {
    try{
        const response = await storageClient.post("/User/GetAllForAdmin", bodyUsers, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const getUsers = async(bodyUsers) => {
    try{
        const response = await storageClient.post("/User", bodyUsers, {withCredentials:true})
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(e){
        return({isSuccess:false, error:e, data: null});
    }
}
export const getSafes = async(code) => {
    try{
        const response = await storageClient.get(`/Storage?code=${code}`, {withCredentials:true});
        // const response = await axios.get("http://localhost:5121/api/v1/Storage",{withCredentials: true});
        // debugger;
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(e){
        // console.log(e);
        return({isSuccess:false, error:e, data: null});
    }
}
export const getSafe = async({uid}) =>{
    try{
        const response = await storageClient.get(`/Storage/${uid}`, {withCredentials:true});
        if(response.status==200){
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(err){
        // console.log(err);
        return({isSuccess:false, error:err, data: null});
    }
}
export const getPersonSafe = async() => {
    try {
        const response = await storageClient.get(`/Storage/my`, {withCredentials:true});
        if(response.status==200){
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(err){
        return({isSuccess:false, error:err, data: null}) 
    }
}
export const getOrganizationSafe = async() => {
    try {
        const response = await storageClient.get(`/Storage/common`, {withCredentials:true});
        if(response.status==200){
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }
    catch(err){
        return({isSuccess:false, error:err, data: null}) 
    }
}
export const getSecrets = async({uid})=>{
    try{
        const response = await storageClient.get(`/Secret/all/${uid}`,{
            withCredentials: true
    });
        // const responce = await axios.get(`http://localhost:7069/api/v1/Secret/all/${param.uid}`,{withCredentials: true});
        // debugger;
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
            // console.log(responce);
        }
    }catch(e){
        // console.log(e);
        return({isSuccess:false, error:e, data: null});

    }
}
export const getSecret = async({uid})=>{
    try{
        const response = await storageClient.get(`/Secret/${uid}`, {withCredentials:true});
        if(response.status==200){
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(err){
        // console.log(err);
        return({isSuccess:false, error:err, data: null});
    }
}