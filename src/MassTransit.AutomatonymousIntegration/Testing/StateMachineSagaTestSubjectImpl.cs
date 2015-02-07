// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous.Testing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using MassTransit.Testing.Configurators;
    using MassTransit.Testing.ScenarioBuilders;
    using MassTransit.Testing.ScenarioConfigurators;
    using MassTransit.Testing.Subjects;
    using MassTransit.Testing.TestDecorators;
    using RepositoryConfigurators;


    public class StateMachineSagaTestSubjectImpl<TScenario, TSaga, TStateMachine> :
        SagaTestSubject<TSaga>,
        IScenarioSpecification<TScenario>
        where TSaga : class, SagaStateMachineInstance
        where TScenario : ITestScenario
        where TStateMachine : StateMachine<TSaga>
    {
        readonly Action<StateMachineSagaRepositoryConfigurator<TSaga>> _configureCorrelation;
        readonly ISagaRepository<TSaga> _sagaRepository;
        readonly TStateMachine _stateMachine;
        SagaListImpl<TSaga> _created;
        ReceivedMessageList _received;
        SagaListImpl<TSaga> _sagas;

        public StateMachineSagaTestSubjectImpl(ISagaRepository<TSaga> sagaRepository, TStateMachine stateMachine,
            Action<StateMachineSagaRepositoryConfigurator<TSaga>> configureCorrelation)
        {
            _sagaRepository = sagaRepository;
            _stateMachine = stateMachine;
            _configureCorrelation = configureCorrelation;
        }

        public ITestScenarioBuilder<TScenario> Configure(ITestScenarioBuilder<TScenario> builder)
        {
            _received = new ReceivedMessageList(builder.Timeout);
            _created = new SagaListImpl<TSaga>(builder.Timeout);
            _sagas = new SagaListImpl<TSaga>(builder.Timeout);

            var decoratedSagaRepository = new SagaRepositoryTestDecorator<TSaga>(_sagaRepository, _received, _created,
                _sagas);
            var scenarioBuilder = builder as IBusTestScenarioBuilder;
            if (scenarioBuilder != null)
                scenarioBuilder.ConfigureReceiveEndpoint(
                    x => x.StateMachineSaga(_stateMachine, decoratedSagaRepository, _configureCorrelation));

            return builder;
        }

        public IEnumerable<TestConfiguratorResult> Validate()
        {
            yield break;
        }

        public IReceivedMessageList Received
        {
            get { return _received; }
        }

        public ISagaList<TSaga> Created
        {
            get { return _created; }
        }

        public void Dispose()
        {
        }

        public IEnumerator<ISagaInstance<TSaga>> GetEnumerator()
        {
            return _sagas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<ISagaInstance<TSaga>> Select()
        {
            return _created.Select();
        }

        public IEnumerable<ISagaInstance<TSaga>> Select(Func<TSaga, bool> filter)
        {
            return _created.Select(filter);
        }

        public TSaga Contains(Guid sagaId)
        {
            return _created.Contains(sagaId);
        }
    }
}