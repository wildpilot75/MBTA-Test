import React, { Component } from 'react';

export class MbtaBoard extends Component {
    static displayName = MbtaBoard.name;
    intervalID;

    constructor(props) {
        super(props);
        this.state = { northstationschedule: [], loading: true };
        this.state = { southstationschedule: [], loading: true };
    }

    componentDidMount() {
        this.populateBoardData();

        this.intervalID = setInterval(this.populateBoardData.bind(this), 5000);
    }

    componentWillUnmount() {
        clearInterval(this.intervalID);
    }

    static renderScheduleInfoBoard(scheduleItems) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Direction</th>
                        <th>Carrier</th>
                        <th>Time</th>
                        <th>Destination</th>
                        <th>Train#</th>
                        <th>Track#</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    {scheduleItems.map(scheduleItem =>
                        <tr>
                            <td>{scheduleItem.direction}</td>
                            <td>{scheduleItem.carrier}</td>
                            <td>{scheduleItem.time}</td>
                            <td>{scheduleItem.destination}</td>
                            <td>{scheduleItem.train}</td>
                            <td>{scheduleItem.track}</td>
                            <td>{scheduleItem.status}</td>
                        </tr>
                    )}
                </tbody>
            </table>
            );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : MbtaBoard.renderScheduleInfoBoard(this.state.southstationschedule);
        let contents1 = this.state.loading
            ? <p><em>Loading...</em></p>
            : MbtaBoard.renderScheduleInfoBoard(this.state.northstationschedule);
        return (
            <div>
                <h1 id="tabelLabel" >South Station Arrival/Depatrue Board</h1>
                {contents}
                <h1 id="tabelLabel" >North Station Arrival/Depatrue Board</h1>
                {contents1}
            </div>
        );
    }

    async populateBoardData() {
        const response = await fetch('mbtainfoboard/North Station');
        const data = await response.json();
        this.setState({ northstationschedule: data, loading: false });

        const response1 = await fetch('mbtainfoboard/South Station');
        const data1 = await response1.json();
        this.setState({ southstationschedule: data1, loading: false });
    }
}