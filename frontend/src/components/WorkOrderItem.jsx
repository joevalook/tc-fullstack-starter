function WorkOrderItem({ workOrder }) {
  return (
    <li className="card">
      <h3>{workOrder.title}</h3>
      <p>Status: {workOrder.status}</p>
      {workOrder.description && (
        <p>Description: {workOrder.description}</p>
      )}
    </li>
  );
}

export default WorkOrderItem;