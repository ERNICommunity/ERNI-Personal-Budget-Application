import { MenuItem } from "primeng/api";

export class MenuHelper {
  public static getYearMenu(
    linkBuilder: (number: number) => (string | number)[]
  ): MenuItem[] {
    const currentYear = new Date().getFullYear();

    const years = [];

    for (let year = currentYear; year >= 2015; year--) {
      years.push({
        label: year.toString(),
        routerLink: linkBuilder(year),
      });
    }

    return years;
  }
}
