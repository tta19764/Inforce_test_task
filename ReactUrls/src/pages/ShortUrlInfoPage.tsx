import { useParams } from "react-router-dom";
import type { IUrlsService } from "../interfaces/IUrlsService";
import { urlsService } from "../services/urlsService";
import { useEffect, useState } from "react";
import type { Url } from "../types/Url";
import { useAppSelector } from "../hooks/hooks";
import '../styles/shorturlinfopage.css';

function ShortUrlInfoPage(){
    const apiUrlService: IUrlsService = urlsService;

    const { urlId } = useParams();
    const { isLoggedIn } = useAppSelector((state) => state.user);

    const [url, setUrl] = useState<Url | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!urlId || !isLoggedIn) return;

        const loadData = async () => {
            try {
                setIsLoading(true);
                const result = await apiUrlService.getUrl(Number(urlId));

                if (!result) {
                    setError("URL not found.");
                    return;
                }

                setUrl(result);
            } catch {
                setError("Failed to load URL details.");
            } finally {
                setIsLoading(false);
            }
        };

        loadData();
    }, [urlId, isLoggedIn]);

    if (!isLoggedIn) {
        return <p className="url-info-error">You must be logged in to view this page.</p>;
    }

    if (isLoading) {
        return <p className="url-info-loading">Loading...</p>;
    }

    if (error) {
        return <p className="url-info-error">{error}</p>;
    }

    if (!url) {
        return null;
    }

    return (
        <div className="url-info-container">
            <h2>Short URL Information</h2>

            <div className="url-info-row">
                <span>ID</span>
                <span>{url.id}</span>
            </div>

            <div className="url-info-row">
                <span>Original URL</span>
                <a href={url.originalUrl} target="_blank" rel="noopener noreferrer">
                    {url.originalUrl}
                </a>
            </div>

            <div className="url-info-row">
                <span>Short URL</span>
                <span>
                    {url.shortUrl}
                </span>
            </div>

            <div className="url-info-row">
                <span>Created By</span>
                <span>{url.creatorNickname}</span>
            </div>

            <div className="url-info-row">
                <span>Clicks</span>
                <span>{url.clickCount}</span>
            </div>

            <div className="url-info-row">
                <span>Created At</span>
                <span>{new Date(url.createdAt).toLocaleString()}</span>
            </div>
        </div>
    );
}

export default ShortUrlInfoPage;