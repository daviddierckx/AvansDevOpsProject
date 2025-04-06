namespace AvansDevOps.App.Domain.Entities
{
    // Abstracte basisklasse voor alle pipeline acties
    // Dit zou deel kunnen zijn van een Command Pattern als je undo/redo wilde,
    // maar hier functioneert het meer als een basis type.
    // Ook gerelateerd aan het Template Method pattern als Execute een vaste structuur had
    // met aanpasbare stappen, maar hier is Execute volledig abstract.
    public abstract class PipelineAction
    {
        public string Name { get; protected set; }

        protected PipelineAction(string name)
        {
            Name = name;
        }

        // Abstracte methode die door subclasses geïmplementeerd moet worden
        // Retourneert true bij succes, false bij falen.
        public abstract bool Execute();
    }
}