import axios from "axios";
import { userStorage } from "../main";
import type { RefreshTokenRequest } from "../types/RefreshTokenRequest";
const BASE_URL = import.meta.env.VITE_BASE_API;
const AUTH_URL = import.meta.env.VITE_APP_AUTH_ENDPOINT;

export const axiosPublic = axios.create({
    baseURL: BASE_URL
});

export const axiosPrivate = axios.create({
    baseURL: BASE_URL,
    headers: { 'Content-Type': 'application/json' },
});

axiosPrivate.interceptors.request.use(config => {
    const token = userStorage.getToken();
    if (token && !config.headers.Authorization) {
        config.headers.Authorization = `Bearer ${token.accessToken}`;
    }

    return config;
});

const refresh = async (token: RefreshTokenRequest) => {
    console.log("Refreshing token...");
    const response = await axiosPublic.post(AUTH_URL + 'refresh-token', JSON.stringify(token),{
        headers: {
            "Content-Type": "application/json",
            credentials: 'include'
        },
    });

    userStorage.setToken(response.data);

    return response.data.accessToken;
};

axiosPrivate.interceptors.response.use(
    response => response,
    async error => {
        const prevRequest = error?.config;
        if (!prevRequest) {
            return Promise.reject(error);
        }

        const token = userStorage.getToken();
        const user = userStorage.getUser();

    if (
        error?.response?.status === 401 &&
        !prevRequest?.sent &&
        token?.refreshToken &&
        user?.userId
    ) {
        prevRequest.sent = true;
        try{
            const refreshTokenRequest: RefreshTokenRequest = {
                userId: user.userId,
                refreshToken: token.refreshToken
            };

            const newToken = await refresh(refreshTokenRequest);
            prevRequest.headers.Authorization = `Bearer ${newToken}`;
            return axiosPrivate(prevRequest);
        } catch (err) {
            userStorage.clearUser();
            return Promise.reject(err);
        }
    }

    return Promise.reject(error);
});
