import { Suspense } from "react";
import BriefPageClient from "./BriefPageClient";

export default function BriefPage() {
    return(
        <Suspense fallback={ <p className="p-8 text-gray-500">Loading...</p>}>
            <BriefPageClient />
        </Suspense>
    );
}
