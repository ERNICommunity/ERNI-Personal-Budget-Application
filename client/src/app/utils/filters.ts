import { Request } from "../model/request/request";

export const filterRequests = (
  requests: Request[],
  searchString: string
): Request[] => {
  searchString = searchString
    .toLowerCase()
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "");

  return requests.filter(
    (request) =>
      request.user.firstName
        .toLowerCase()
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .indexOf(searchString) !== -1 ||
      request.user.lastName
        .toLowerCase()
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .indexOf(searchString.toLowerCase()) !== -1
  );
};
