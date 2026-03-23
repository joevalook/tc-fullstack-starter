import { getToken } from "../utils/auth";

const API_BASE_URL = "http://localhost:5257";

export async function getWorkOrders() {
  const token = getToken();

  const response = await fetch(`${API_BASE_URL}/api/workorders`, {
    headers: token
      ? {
          Authorization: `Bearer ${token}`,
        }
      : {},
  });

  if (!response.ok) {
    throw new Error("Failed to fetch work orders");
  }

  return response.json();
}