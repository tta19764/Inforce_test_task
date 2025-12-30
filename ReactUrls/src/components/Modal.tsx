import { useState } from "react";
import { closeModal, setUpdateTable } from "../features/modal/modalSlice";
import { useDispatch } from "react-redux";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
const URL_URL = import.meta.env.VITE_APP_URLS_ENDPOINT;

function Modal() {
    const controller = new AbortController();
    const dispatch = useDispatch();
    const [url, setUrl] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const axiosPrivate = useAxiosPrivate();

    const handleSubmit = async () => {
        if (!url.trim()) {
            setError("URL is required.");
            return;
        }

        try {
            setIsSubmitting(true);
            setError(null);

            const response = await axiosPrivate.post(URL_URL, url,{
                    signal: controller.signal
                });

            if (!response.data) {
                setError("Failed to add URL. It may already exist.");
                return;
            }

            dispatch(setUpdateTable());
            dispatch(closeModal());
        } catch (error) {
            console.error(error);
            setError("Failed to add URL. It may already exist.");
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <aside className="modal-container">
            <div className="modal-div">
                <h2>Add new URL</h2>

                <input
                    type="text"
                    className="url-input"
                    placeholder="https://example.com"
                    value={url}
                    onChange={(e) => setUrl(e.target.value)}
                    disabled={isSubmitting}
                />

                {error && <p className="error-text">{error}</p>}

                <div className="btn-container">
                    <button
                        type="button"
                        className="btn confirm-btn"
                        onClick={handleSubmit}
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? "Adding..." : "Confirm"}
                    </button>

                    <button
                        type="button"
                        className="btn clear-btn"
                        onClick={() => dispatch(closeModal())}
                        disabled={isSubmitting}
                    >
                        Cancel
                    </button>
                </div>
            </div>
        </aside>
    );
}

export default Modal;