import { Request } from "../model/request/request";
import { normalize } from "./normalizer.util";

export const filterRequests = (
  requests: Request[],
  searchString: string
): Request[] => {
  searchString = normalize(searchString);

  return requests.filter(
    (request) =>
      normalize(request.user.firstName).indexOf(searchString) !== -1 ||
      normalize(request.user.lastName).indexOf(searchString.toLowerCase()) !== -1
  );
};
