import type { Url } from "../types/Url";
import type { UrlTableRequest } from "../types/UrlTableRequest";

export interface IUrlsService {
    getUrlsCount(signal?: AbortSignal): Promise<number>;
    getUrl(urlId: number, signal?: AbortSignal): Promise<Url | null>;
    getUrls(request: UrlTableRequest, signal?: AbortSignal): Promise<Url[]>;
    deleteUrl(urlId: number, signal?: AbortSignal): Promise<boolean>;
    addUrl(url: string, signal?: AbortSignal): Promise<Url | null>;
}