export class NavMock {
  public pop(): any {
    return new Promise(function (resolve: Function): void {
      resolve('POP');
    });
  }

  public push(): any {
    return new Promise(function (resolve: Function): void {
      resolve('PUSH');
    });
  }

  public getActive(): any {
    return {
      'instance': {
        'model': 'something',
      },
    };
  }

  public setRoot(): any {
    return true;
  }

  public registerChildNav(nav: any): void {
    return;
  }
}
