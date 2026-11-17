const API = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5247";

//Register a new user
export async function register(email: string, password: string) {
    const res = await fetch(`${API}/account/register`, {
        method: "POST",
        headers: {"Content-Type": "application/json" },
        body: JSON.stringify({email, password})
    });

    if (!res.ok) throw new Error(await res.text());
}

//Login
export async function login(email: string, password: string) {
    const res = await fetch(`${API}/account/login?useCookies=false&useSessionCookies=false`, {
        method: "POST",
        headers: {"Content-Type": "application/json" },
        body: JSON.stringify({email, password})
    });

    if(!res.ok) throw new Error(await res.text());

    const data = await res.json();
    return data.accessToken as string;
}

//API call with presets
export async function apiFetch(path: string, options: RequestInit = {}){
    const token = typeof window !== "undefined" ? localStorage.getItem("bq_token") : null;

    const isFormData = options.body instanceof FormData;

    const res = await fetch(`${API}${path}`, {
        ...options,
        headers: {
            ...(isFormData ? {} : {"Content-Type": "application/json"}),
            ...(token ? {Authorization: `Bearer ${token}` } : {}),
            ...(options.headers ?? {}),
        } 
    });

    //If user is unauthorised, send back to sign in page
    if(res.status === 401){
        localStorage.removeItem("bq_token");
        window.location.href = "/auth";
        throw new Error("Session expired.");
    }

    if (!res.ok) {
    let message = `${res.status}: Request failed`;
    try {
        const contentType = res.headers.get("content-type") ?? "";
        if (contentType.includes("application/json") || contentType.includes("application/problem+json")) {
        const body = await res.json();
        message = body.detail ?? body.title ?? body.message ?? message;
        } else {
        message = await res.text() || message;
        }
    } catch {
        // If parsing fails, keep the status-based message
    }
    throw new Error(`${res.status}: ${message}`);
    }
    return res;
}

//GET /briefs
export async function getBriefs(){
    const res = await apiFetch("/briefs");
    return res.json();
}

//POST new brief
export async function createBrief(title: string, text: string){
    const res = await apiFetch("/briefs", {
        method: "POST",
        body: JSON.stringify({ title, text })
    });

    return res.json();
}


//POST brief to Gemini: Extract it
export async function extractBrief(id: number){
    const res = await apiFetch(`/briefs/${id}/extract`, {method: "POST"});
    return res.json();
}

//GET brief/id
export async function getBriefDetail(id: number){
    const res = await apiFetch(`/briefs/${id}`);
    return res.json();
}

//PATCH quest status
export async function patchQuestStatus(briefID: number, questID: number, status: string){
    const res = await apiFetch(`/briefs/${briefID}/quests/${questID}`, {
        method: "PATCH",
        body : JSON.stringify({ status })
    });

    return res.json();
}