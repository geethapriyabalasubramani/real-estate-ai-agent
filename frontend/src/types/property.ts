export interface PropertyListItem {
    id: string;
    addressLine1: string;
    city: string;
    state: string;
    postalCode: string;
    bedrooms: number;
    bathrooms: number;
    price: number;
    squareFeet: number;
    hasGarage: boolean;
  }
  
  export interface PagedPropertyResponse {
    items: PropertyListItem[];
    page: number;
    pageSize: number;
    totalCount: number;
  }