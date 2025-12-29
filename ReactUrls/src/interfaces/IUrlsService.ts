import type { Url } from "../types/Url";
import type { UrlTableRequest } from "../types/UrlTableRequest";

export interface IUrlsService {
    getUrlsCount(): Promise<number>;
    getUrl(urlId: number) : Promise<Url | null>;
    getUrls(request: UrlTableRequest) : Promise<Url[]>;
    deleteUrl(urlId: number) : Promise<boolean>;
    addUrl(url: string) : Promise<Url | null>;
}