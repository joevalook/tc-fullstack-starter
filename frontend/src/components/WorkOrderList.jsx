import WorkOrderItem from "./WorkOrderItem";

function WorkOrderList({ workOrders, loading }) {
  if (loading) {
    return <p>Loading work orders...</p>;
  }

  if (workOrders.length === 0) {
    return <p>No work orders found.</p>;
  }

  return (
    <ul className="workorder-list">
      {workOrders.map((wo) => (
        <WorkOrderItem key={wo.id} workOrder={wo} />
      ))}
    </ul>
  );
}

export default WorkOrderList;