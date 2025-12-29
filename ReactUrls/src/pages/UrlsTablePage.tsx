import { useEffect, useState } from "react";
import type { UrlTableRequest } from "../types/UrlTableRequest";
import type { IUrlsService } from "../interfaces/IUrlsService";
import { urlsService } from "../services/urlsService";
import type { Url } from "../types/Url";
import '../styles/urlstablepage.css';
import { useAppSelector } from "../hooks/hooks";
import { useDispatch } from "react-redux";
import { openModal, setUpdateTable } from "../features/modal/modalSlice";
import Modal from "../components/Modal";
import { Link, useParams } from "react-router-dom";

function UrlsTablePage() {
    const { page } = useParams();
    const dispatch = useDispatch();
    const { isOpen, updateTable } = useAppSelector((state) => state.modal);
    const { isLoggedIn, userData } = useAppSelector((state) => state.user);
    const apiUrlService: IUrlsService = urlsService;

    const [urls, setUrls] = useState<Url[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [urlCount, setUrlCount] = useState(0);
    const pageNumber = page ? Number.parseInt(page) : 1;

    const pageSize = 10;

    useEffect(() => {
        const loadData = async () => {
            setIsLoading(true);

            const request: UrlTableRequest = {
                pageNumber,
                pageSize,
            };

            const [urlsData, count] = await Promise.all([
                apiUrlService.getUrls(request),
                apiUrlService.getUrlsCount(),
            ]);

            setUrls(urlsData);
            setUrlCount(count);
            setIsLoading(false);
        };

        loadData();
    }, [pageNumber, updateTable]);

    const totalPages = Math.ceil(urlCount / pageSize);

    async function deleteUrlFunc(id:number) {
        await apiUrlService.deleteUrl(id);
        dispatch(setUpdateTable());
    };

    if (isLoading) {
        return <h1>loading...</h1>;
    }

    return (
        <>
        {isOpen && isLoggedIn && <Modal />}
        <div className="urls-container">
            <div className="urls-header">
                <h2>URLs</h2>
                {isLoggedIn && <button 
                className="add-url-btn"
                onClick={() => {dispatch(openModal())}}>+ Add URL</button>}
            </div>

            <table className="urls-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Original URL</th>
                        <th>Short URL</th>
                        {userData && <th>Actions</th>}
                    </tr>
                </thead>
                <tbody>
                    {urls.map((url) => (
                        <tr key={url.id}>
                            <td>{url.id}</td>
                            <td>{url.originalUrl}</td>
                            <td>{url.shortUrl}</td>
                            {userData && (url.creatorId === userData?.userId || userData?.role === "Admin") && <td>
                                    <button 
                                        className="delete-url-btn"
                                        onClick={() => {
                                            deleteUrlFunc(url.id);
                                        }}>Delete</button>
                                </td>}
                        </tr>
                    ))}
                </tbody>
            </table>

            <div className="pagination">
                {Array.from({ length: totalPages }, (_, i) => i + 1).map(page => (
                    <Link
                        key={page}
                        className={`page-btn ${page === pageNumber ? "active" : ""}`}
                        to={"/urls/"+page.toString()}
                    >
                        {page}
                    </Link>
                ))}
            </div>
        </div>
        </>
    );
}

export default UrlsTablePage;
