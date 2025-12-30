import axiosPublic from "../api/axios";
import type { IUrlsService } from "../interfaces/IUrlsService";
import type { Url } from "../types/Url";
import type { UrlTableRequest } from "../types/UrlTableRequest";

const URL_URL = import.meta.env.VITE_APP_URLS_ENDPOINT;

export const urlsService: IUrlsService = {
    async getUrlsCount(): Promise<number> {
        const res = await axiosPublic.get<number>(`${URL_URL}Count`);
        return res.data;
    },

    async getUrls(request: UrlTableRequest): Promise<Url[]> {
        const res = await axiosPublic.get<Url[]>(
            `${URL_URL}${request.pageNumber}/${request.pageSize}`
        );
        return res.data;
    },
}