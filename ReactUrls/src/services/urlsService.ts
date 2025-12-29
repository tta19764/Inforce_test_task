import axios from "axios";
import axiosCustom from "../api/axios";
import type { IUrlsService } from "../interfaces/IUrlsService";
import type { Url } from "../types/Url";
import type { UrlTableRequest } from "../types/UrlTableRequest";

const URL_URL = import.meta.env.VITE_APP_URLS_ENDPOINT;

export const urlsService: IUrlsService = {
    async getUrlsCount(): Promise<number> {
        const res = await axiosCustom.get<number>(`${URL_URL}Count`);
        return res.data;
    },

    async getUrl(urlId: number): Promise<Url | null> {
        try {
            const res = await axiosCustom.get<Url>(`${URL_URL}${urlId}`);
            return res.data;
        } catch (error) {
            if (axios.isAxiosError(error) && error.response?.status === 404) {
                return null;
            }

            throw error;
        }
    },

    async getUrls(request: UrlTableRequest): Promise<Url[]> {
        const res = await axiosCustom.get<Url[]>(
            `${URL_URL}${request.pageNumber}/${request.pageSize}`
        );
        return res.data;
    },

    async addUrl(url: string): Promise<Url | null> {
        try {
            const res = await axiosCustom.post(URL_URL, url, {
                headers: {
            "Content-Type": "application/json",
        },
            });
            return res.data;
        } catch (error) {
            if (axios.isAxiosError(error) && error.response?.status === 404) {
                return null;
            }

            throw error;
        }

    },
    deleteUrl: async function (urlId: number): Promise<boolean> {
        try{
            await axiosCustom.delete(`${URL_URL}${urlId}`);
            return true;
        } catch {
            return false;
        }
    }
};