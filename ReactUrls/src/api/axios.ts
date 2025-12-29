import axios from "axios";
import type { IUserStorage } from "../interfaces/IUserStorage";
import { userStoreLocalStorage } from "../services/userStoreLocalStorage";
const BASE_URL = import.meta.env.VITE_BASE_API;

const instance = axios.create({
    baseURL: BASE_URL,
});

const userStorage : IUserStorage = userStoreLocalStorage;

instance.interceptors.request.use((config) => {
    console.log("test");
    const user = userStorage.getUser();

    if (user?.token?.accessToken) {
        config.headers.Authorization = `Bearer ${user.token.accessToken}`;
    }

    return config;
});

export default instance;