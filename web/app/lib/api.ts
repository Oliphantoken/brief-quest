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