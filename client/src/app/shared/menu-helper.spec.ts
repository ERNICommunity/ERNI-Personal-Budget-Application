import { MenuHelper } from './menu-helper';

describe('Menu Helper', () => {
  describe('getYearMenu', () => {
    beforeEach(() => {
      jasmine.clock().install();
      jasmine.clock().mockDate(new Date(2024, 8, 10));
    });

    it('should return correct menu items', () => {
      const result = MenuHelper.getYearMenu((year) => ['/my-budget', year]);
      expect(result).toEqual([
        { label: '2024', routerLink: ['/my-budget', 2024] },
        { label: '2023', routerLink: ['/my-budget', 2023] },
        { label: '2022', routerLink: ['/my-budget', 2022] },
        { label: '2021', routerLink: ['/my-budget', 2021] },
        { label: '2020', routerLink: ['/my-budget', 2020] },
        { label: '2019', routerLink: ['/my-budget', 2019] },
        { label: '2018', routerLink: ['/my-budget', 2018] },
        { label: '2017', routerLink: ['/my-budget', 2017] },
        { label: '2016', routerLink: ['/my-budget', 2016] },
        { label: '2015', routerLink: ['/my-budget', 2015] },
      ]);
    });

    afterEach(() => {
      jasmine.clock().uninstall();
    });
  });
});
