// import axios from "axios";
// import { CONFIG } from "../../../shared/config/config";
// import { extractAxiosErrorMessage } from "../../../shared/api/extractAxiosErrorMessage";

// export async function registerScale(
//   hiveId: string,
//   serialNumber: string,
// ): Promise<void> {
//   try {
//     await axios.post(`${CONFIG.SCALES_URL}/register`, { hiveId, serialNumber });
//   } catch (error) {
//     throw new Error(
//       extractAxiosErrorMessage(error, "Failed to register scale."),
//     );
//   }
// }
import axios from "axios";
import { CONFIG } from "../../../shared/config/config";
import type { ApiaryAnalyticsResponse } from "../types/analytics";

export async function registerScale(
  hiveId: string,
  serialNumber: string,
  weightDropLimit?: number,
): Promise<void> {
  try {
    await axios.post(`${CONFIG.SCALES_URL}/register`, {
      hiveId,
      serialNumber,
      weightDropLimit,
    });
  } catch (error: any) {
    console.log("Response podaci sa beka:", error.response?.data);

    if (error.response && error.response.data) {
      const data = error.response.data;

      // 1. Ako je bek poslao objekat { message: "..." } ili { Message: "..." }
      if (data.message || data.Message) {
        throw new Error(data.message || data.Message);
      }

      // 2. Ako je bek poslao "goli" string bez JSON strukture (kako je bilo do sada)
      if (typeof data === "string") {
        throw new Error(data);
      }

      // 3. Ako je bek poslao objekat { error: "..." }
      if (data.error) {
        throw new Error(
          typeof data.error === "string" ? data.error : data.error.message,
        );
      }
    }

    throw new Error(
      "Failed to register scale. Device might be already paired.",
    );
  }
}

export async function getApiaryAnalytics(
  apiaryId: string,
  days: number = 7,
): Promise<ApiaryAnalyticsResponse> {
  try {
    const response = await axios.get<ApiaryAnalyticsResponse>(
      `${CONFIG.SCALES_URL}/analytics/apiary/${apiaryId}`,
      { params: { days } },
    );
    return response.data;
  } catch (error: any) {
    console.error("Greška pri povlačenju analitike sa bekenda:", error);
    throw new Error(error.response?.data || "Failed to load apiary analytics.");
  }
}

export async function saveYieldRecord(payload: {
  scaleId: string;
  deltaKg: number;
  recordedAt: string;
}): Promise<void> {
  try {
    console.log("[API] Sending YieldRecord:", payload);
    const response = await axios.post(
      `${CONFIG.SCALES_URL.replace("/SmartScales", "")}/yieldrecords/save`,
      payload,
    );
    console.log("[API] YieldRecord saved successfully:", response.data);
  } catch (error: any) {
    console.error("[API] Greška pri spravljanju yield record-a:", error);
    console.error("[API] Response status:", error.response?.status);
    console.error("[API] Response data:", error.response?.data);
    throw new Error(error.response?.data || "Failed to save yield record.");
  }
}
