import axios from "axios";
import { toRefresh } from "./authClient";

axios.defaults.withCredentials = true;

export const auditClient = axios.create({
    baseURL:"https://localhost:7005/api/v1",
    withCredentials: true,
    headers: "Access-Control-Allow-Origin"
})

auditClient.interceptors.response.use(async(response) => {
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
export const getAllRecordsForAdmin = async({bodyHistory}) => {
    axios.defaults.withCredentials = true;
    try{
        const response = await auditClient.post("https://localhost:7005/api/v1/Audit/GetAllRecordsForAdmin", bodyHistory, {

            withCredentials: true
        });
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(e){
        // console.log(e);
        return({isSuccess:false, error:e, data: null});
    }
}
export const getHistory = async({bodyHistory}) => {
    axios.defaults.withCredentials = true;
    try{
        const response = await auditClient.post("https://localhost:7005/api/v1/Audit", bodyHistory, {

            withCredentials: true
        });
        if (response.status==200)
        {
            return({isSuccess:true, error:null, data: response.data.data});
        }
    }catch(e){
        // console.log(e);
        return({isSuccess:false, error:e, data: null});
    }
}
