import axios from "axios";
import { axiosPublic, axiosPrivate } from "../api/axios";
import type { IUrlsService } from "../interfaces/IUrlsService";
import type { Url } from "../types/Url";
import type { UrlTableRequest } from "../types/UrlTableRequest";

const URL_URL = import.meta.env.VITE_APP_URLS_ENDPOINT;

export const urlsServiceDotNet: IUrlsService = {
    async getUrlsCount(signal?: AbortSignal): Promise<number> {
        try {
            const res = await axiosPublic.get<number>(
                `${URL_URL}Count`,
                { signal }
            );
            return res.data;
        } catch (error) {
            if (axios.isCancel(error)) throw error;
            console.error("getUrlsCount failed", error);
            throw error;
        }
    },

    async getUrls(
        request: UrlTableRequest,
        signal?: AbortSignal
    ): Promise<Url[]> {
        try {
            const res = await axiosPublic.get<Url[]>(
                `${URL_URL}${request.pageNumber}/${request.pageSize}`,
                { signal }
            );
            return res.data;
        } catch (error) {
            if (axios.isCancel(error)) throw error;
            console.error("getUrls failed", error);
            throw error;
        }
    },

    async getUrl(
        urlId: number,
        signal?: AbortSignal
    ): Promise<Url | null> {
        try {
            const res = await axiosPrivate.get<Url | null>(
                `${URL_URL}${urlId}`,
                { signal }
            );
            return res.data;
        } catch (error) {
            if (axios.isCancel(error)) throw error;
            console.error("getUrl failed", error);
            throw error;
        }
    },

    async deleteUrl(
        urlId: number,
        signal?: AbortSignal
    ): Promise<boolean> {
        try {
            const res = await axiosPrivate.delete<boolean>(
                `${URL_URL}${urlId}`,
                { signal }
            );
            return res.data;
        } catch (error) {
            if (axios.isCancel(error)) throw error;
            console.error("deleteUrl failed", error);
            throw error;
        }
    },

    async addUrl(
        url: string,
        signal?: AbortSignal
    ): Promise<Url | null> {
        try {
            const res = await axiosPrivate.post<Url | null>(
                URL_URL,
                { originalUrl: url },
                { signal }
            );
            return res.data;
        } catch (error) {
            if (axios.isCancel(error)) throw error;
            console.error("addUrl failed", error);
            throw error;
        }
    }
}