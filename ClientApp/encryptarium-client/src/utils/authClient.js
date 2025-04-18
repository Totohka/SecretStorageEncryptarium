import axios from "axios";
import { data, useNavigate } from "react-router";
axios.defaults.withCredentials = true;
const authClient = axios.create({
    baseURL:'https://localhost:7083/api/v1',
    withCreditals: true,
    headers: "Access-Control-Allow-Origin"
})

authClient.interceptors.response.use(async(response) => {
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
// authClient.interceptors.response.use((response) => {
//     return response;
// }, (error) => {
//     if (error.response.status === 500) {
//         // If the error has status code 429, retry the request
//         toRefresh();
//         return axios.request(error.config);
//     }
//     return Promise.reject(error);
// });
export const toAuthorize = async({login, pass})=>{
    console.log(login, pass);
    try{
        const responce = await authClient.post(
            "/UserPass/Login", 
            // "http://localhost:5146/api/v1/UserPass/Login", 
            {
            login: login,
            password: pass
        });
        console.log(responce);
        if (responce.status==200){
            return({isSuccess:true, error:null});
        }
    }catch(err){
        console.log(err);
        return({isSuccess:false, error:err});
    }
}
export const toSendCode = async({login, pass, code})=>{
    try{
        const responce = await authClient.post(
            "/UserPass/VerifyCode", 
            // "http://localhost:5146/api/v1/UserPass/VerifyCode", 
            {
            login: login,
            password: pass,
            code: code
        });
        console.log(responce);
        if (responce.status==200){
            return({isSuccess:true, error:null});
        }
    }catch(err){
        return({isSuccess:false, error:err});
    }
}
export const toSignUp = async({email, login, pass}) =>{
    try{
        const responce = await authClient.post("/UserPass/Registration", {
            email: email,
            login: login,
            password: pass
        });
        if (responce.status==200){
            return({isSuccess:true, error:null});
        }
    }catch(err){
        console.log(err);
        return({isSuccess:false, error:err});
    }
}
export const getApiKeysByUserUid = async(userUid) => {
    try {
        const responce = await authClient.get(`/ApiKey/GetApiKeysByUserUid?userUid=${userUid}`, {withCreditals:true});
        if (responce.status==200){
            return({isSuccess:true, error:null, data: responce.data});
        }
    } catch (err) {
        console.log(err);
        return({isSuccess:false, error:err});
    }
}
export const deactivedApiKeysByUid = async(apikeyUid, isAll) => {
    try {
        const responce = await authClient.put(`/ApiKey/DeactivedApiKey?apiKeyUid=${apikeyUid}&isAll=${isAll}`, undefined ,{withCreditals:true});
        if (responce.status==200){
            return({isSuccess:true, error:null, data: responce.data});
        }
    } catch (err) {
        console.log(err);
        return({isSuccess:false, error:err});
    }
}
export const deactivedIpByUid = async(ipUid) => {
    try {
        const responce = await authClient.put(`/WhiteIp/deactivate/${ipUid}`, undefined ,{withCreditals:true});
        if (responce.status==200){
            return({isSuccess:true, error:null, data: responce.data});
        }
    } catch (err) {
        console.log(err);
        return({isSuccess:false, error:err});
    }
}

export const getIpsByApiKeyUid = async(apiKeyUid) => {
    try {
        const responce = await authClient.get(`/WhiteIp/GetIpByApiKey?apiKeyUid=${apiKeyUid}`, {withCreditals:true});
        if (responce.status==200){
            return({isSuccess:true, error:null, data: responce.data});
        }
    } catch (err) {
        console.log(err);
        return({isSuccess:false, error:err});
    }
}
export const toRefresh = async() => {
    try{
        const responce = await authClient.post("/Token/RefreshToken", {withCreditals:true});
        if (responce.status==200){
            return({isSuccess:true, error:null});
        }
        // else{
        //     return({isSuccess: false, error: null});
        // }
    }catch(err){
        console.log(err);
        return({isSuccess:false, error:err});
    }
}