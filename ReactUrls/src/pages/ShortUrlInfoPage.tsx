import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import type { Url } from "../types/Url";
import { useAppSelector } from "../hooks/reduxHooks";
import '../styles/shorturlinfopage.css';
import axios from "axios";
import { urlsService } from "../main";

function ShortUrlInfoPage(){
    const { urlId } = useParams();
    const { isLoggedIn } = useAppSelector((state) => state.user);

    const [url, setUrl] = useState<Url | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!urlId || !isLoggedIn) return;
        let didCancel = false;
        const controller = new AbortController();

        const loadData = async () => {
            try {
                setIsLoading(true);
                const response = await urlsService.getUrl(Number.parseInt(urlId), controller.signal);

                setUrl(response);
            } catch (error) {
                if (axios.isAxiosError(error) && error.code === "ERR_CANCELED") return;
                setError("Failed to load URL details.");
            }finally {
                if (!didCancel) setIsLoading(false);
            }
        }

        loadData();

        return () => {
            didCancel = true;
            controller.abort();
        };
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