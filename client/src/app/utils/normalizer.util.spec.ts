import { normalize } from './normalizer.util';

describe('Normalizer util', () => {
  it('normalize string', () => {
    expect(normalize('')).toEqual('');
    expect(normalize('A')).toEqual('a');
    expect(normalize('Martin')).toEqual('martin');
    expect(normalize('MÄSO')).toEqual('maso');
    expect(normalize('Šúrek')).toEqual('surek');
    expect(normalize('Thômäs')).toEqual('thomas');
    expect(normalize('Müller')).toEqual('muller');
  });
});
