import type { Url } from "../types/Url";
import type { UrlTableRequest } from "../types/UrlTableRequest";

export interface IUrlsService {
    getUrlsCount(): Promise<number>;
    getUrls(request: UrlTableRequest) : Promise<Url[]>;
}